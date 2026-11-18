using System.Globalization;
using System.Text;
using Xunit;

namespace JpCalendar.Test;

/// <summary>
/// Loads the official Cabinet Office national holiday CSV
/// (https://www8.cao.go.jp/chosei/shukujitsu/syukujitsu.csv) shipped with the tests.
/// </summary>
public sealed class FileFixture : IAsyncLifetime
{
    internal const string JapaneseNationalHolidayDataFileName = "syukujitsu.csv";

    internal const string ExpectedHeader = "国民の祝日・休日月日,国民の祝日・休日名称";

    /// <summary>First date covered by the checked fixture range (inclusive).</summary>
    public static readonly DateOnly MinimumDate = new(1955, 1, 1);

    /// <summary>
    /// Last date covered by the checked fixture range (inclusive). The CSV ends on this date,
    /// which is not the end of the calendar year 2027.
    /// </summary>
    public static readonly DateOnly MaximumDate = new(2027, 11, 23);

    private static readonly Encoding ShiftJisEncoding = CreateShiftJisEncoding();

    private readonly List<JapaneseNationalHolidayData> japaneseNationalHolidayDataList = new();

    public IReadOnlyList<JapaneseNationalHolidayData> JapaneseNationalHolidayDataList => this.japaneseNationalHolidayDataList;

    public async ValueTask InitializeAsync()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, JapaneseNationalHolidayDataFileName);

        Assert.True(File.Exists(filePath), $"National holiday CSV was not found: {filePath}");

        var bytes = await File.ReadAllBytesAsync(filePath).ConfigureAwait(false);

        using var stream = new MemoryStream(bytes, writable: false);
        using var streamReader = new StreamReader(
            stream,
            ShiftJisEncoding,
            detectEncodingFromByteOrderMarks: false);

        this.japaneseNationalHolidayDataList.AddRange(Parse(streamReader));
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Parses and validates the official CSV format: a fixed header followed by
    /// strictly ascending <c>yyyy/M/d,name</c> records.
    /// </summary>
    internal static IReadOnlyList<JapaneseNationalHolidayData> Parse(TextReader reader)
    {
        var header = reader.ReadLine();

        if (header is null)
        {
            throw new InvalidDataException("The national holiday CSV is empty.");
        }

        header = header.TrimStart('\uFEFF');

        if (header != ExpectedHeader)
        {
            throw new InvalidDataException($"Unexpected CSV header: {header}");
        }

        var result = new List<JapaneseNationalHolidayData>();
        DateOnly? previousDate = null;
        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            if (line.Length == 0)
            {
                throw new InvalidDataException("The national holiday CSV contains an empty line.");
            }

            var columns = line.Split(',');

            if (columns.Length != 2)
            {
                throw new InvalidDataException($"Malformed record (expected 2 columns): {line}");
            }

            if (columns[0].Length == 0 || columns[1].Length == 0)
            {
                throw new InvalidDataException($"Malformed record (empty column): {line}");
            }

            if (!DateOnly.TryParseExact(
                    columns[0],
                    "yyyy/M/d",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
            {
                throw new InvalidDataException($"Malformed date (expected yyyy/M/d): {columns[0]}");
            }

            if (date < MinimumDate || date > MaximumDate)
            {
                throw new InvalidDataException(
                    $"Date outside the checked range {MinimumDate:yyyy-MM-dd}..{MaximumDate:yyyy-MM-dd}: {date:yyyy-MM-dd}");
            }

            if (previousDate == date)
            {
                throw new InvalidDataException($"Duplicate record for {date:yyyy-MM-dd}.");
            }

            if (previousDate > date)
            {
                throw new InvalidDataException(
                    $"Records are not sorted ascending: {date:yyyy-MM-dd} follows {previousDate:yyyy-MM-dd}.");
            }

            result.Add(new JapaneseNationalHolidayData
            {
                Date = date,
                Name = columns[1],
            });

            previousDate = date;
        }

        if (result.Count == 0)
        {
            throw new InvalidDataException("The national holiday CSV contains no records.");
        }

        return result;
    }

    private static Encoding CreateShiftJisEncoding()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        return Encoding.GetEncoding(
            932,
            EncoderFallback.ExceptionFallback,
            DecoderFallback.ExceptionFallback);
    }
}
