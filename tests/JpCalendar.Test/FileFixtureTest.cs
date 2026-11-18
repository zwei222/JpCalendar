using System.Globalization;
using System.Security.Cryptography;
using Xunit;

namespace JpCalendar.Test;

public sealed class FileFixtureTest
{
    private const string Header = "国民の祝日・休日月日,国民の祝日・休日名称";

    [Fact(DisplayName = "Rejects a missing header")]
    public void RejectsMissingHeader()
    {
        var csv = $"1955/1/1,元日\n1955/1/2,成人の日\n";

        Assert.Throws<InvalidDataException>(() => FileFixture.Parse(new StringReader(csv)));
    }

    [Fact(DisplayName = "Rejects a wrong header")]
    public void RejectsWrongHeader()
    {
        var csv = $"date,name\n1955/1/1,元日\n";

        Assert.Throws<InvalidDataException>(() => FileFixture.Parse(new StringReader(csv)));
    }

    [Fact(DisplayName = "Rejects an empty file")]
    public void RejectsEmptyFile()
    {
        Assert.Throws<InvalidDataException>(() => FileFixture.Parse(new StringReader(string.Empty)));
    }

    [Fact(DisplayName = "Rejects a header-only file")]
    public void RejectsHeaderOnlyFile()
    {
        Assert.Throws<InvalidDataException>(() => FileFixture.Parse(new StringReader($"{Header}\n")));
    }

    [Theory(DisplayName = "Rejects malformed records")]
    [InlineData("1955/1/1")]
    [InlineData("1955/1/1,元日,extra")]
    [InlineData("1955/1/1,")]
    [InlineData(",元日")]
    [InlineData("1955-01-01,元日")]
    [InlineData("not-a-date,元日")]
    public void RejectsMalformedRecord(string line)
    {
        var csv = $"{Header}\n{line}\n";

        Assert.Throws<InvalidDataException>(() => FileFixture.Parse(new StringReader(csv)));
    }

    [Fact(DisplayName = "Rejects an empty line")]
    public void RejectsEmptyLine()
    {
        var csv = $"{Header}\n1955/1/1,元日\n\n1955/1/2,元日\n";

        Assert.Throws<InvalidDataException>(() => FileFixture.Parse(new StringReader(csv)));
    }

    [Fact(DisplayName = "Rejects duplicate dates")]
    public void RejectsDuplicateDates()
    {
        var csv = $"{Header}\n1955/1/1,元日\n1955/1/1,元日\n";

        Assert.Throws<InvalidDataException>(() => FileFixture.Parse(new StringReader(csv)));
    }

    [Fact(DisplayName = "Rejects unsorted dates")]
    public void RejectsUnsortedDates()
    {
        var csv = $"{Header}\n1955/1/2,元日\n1955/1/1,元日\n";

        Assert.Throws<InvalidDataException>(() => FileFixture.Parse(new StringReader(csv)));
    }

    [Theory(DisplayName = "Rejects dates outside the checked range")]
    [InlineData("1954/12/31,元日")]
    [InlineData("2027/11/24,勤労感謝の日")]
    public void RejectsOutOfRangeDates(string line)
    {
        var csv = $"{Header}\n{line}\n";

        Assert.Throws<InvalidDataException>(() => FileFixture.Parse(new StringReader(csv)));
    }

    [Fact(DisplayName = "Parses valid records")]
    public void ParsesValidRecords()
    {
        var csv = $"{Header}\n1955/1/1,元日\n2027/11/23,勤労感謝の日\n";

        var records = FileFixture.Parse(new StringReader(csv));

        Assert.Equal(2, records.Count);
        Assert.Equal(new DateOnly(1955, 1, 1), records[0].Date);
        Assert.Equal("元日", records[0].Name);
        Assert.Equal(new DateOnly(2027, 11, 23), records[1].Date);
        Assert.Equal("勤労感謝の日", records[1].Name);
    }

    [Fact(DisplayName = "Parses independently of the current culture")]
    public async Task ParsesIndependentlyOfCurrentCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            var japaneseCulture = new CultureInfo("ja-JP");
            japaneseCulture.DateTimeFormat.Calendar = new JapaneseCalendar();
            CultureInfo.CurrentCulture = japaneseCulture;
            CultureInfo.CurrentUICulture = japaneseCulture;

            var fixture = new FileFixture();
            await fixture.InitializeAsync();

            Assert.Equal(1067, fixture.JapaneseNationalHolidayDataList.Count);
            Assert.Equal(FileFixture.MinimumDate, fixture.JapaneseNationalHolidayDataList[0].Date);
            Assert.Equal(FileFixture.MaximumDate, fixture.JapaneseNationalHolidayDataList[^1].Date);
            Assert.True(Calendar.IsNationalHoliday(new DateOnly(2027, 11, 23)));
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    [Fact(DisplayName = "Fixture matches the published checksum and size")]
    public async Task MatchesPublishedChecksumAndSize()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, FileFixture.JapaneseNationalHolidayDataFileName);
        var bytes = await File.ReadAllBytesAsync(filePath, TestContext.Current.CancellationToken);

        Assert.Equal(21_538, bytes.Length);
        Assert.Equal(
            "cec37a743c96995cdb9cb52b685c9003634682a9b0e1a640a6b9b96881fe964a",
            Convert.ToHexStringLower(SHA256.HashData(bytes)));
    }
}
