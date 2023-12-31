﻿using System.Text;
using Xunit;

namespace JpCalendar.Test;

public sealed class FileFixture : IAsyncLifetime
{
    /// <summary>
    /// Download csv from https://www8.cao.go.jp/chosei/shukujitsu/gaiyou.html
    /// </summary>
    private const string JapaneseNationalHolidayDataFilePath = @"./syukujitsu.csv";

    private readonly Encoding shiftJisEncoding;

    public FileFixture()
    {
#if NET6_0_OR_GREATER
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        this.shiftJisEncoding = Encoding.GetEncoding("Shift_JIS");
        this.JapaneseNationalHolidayDataList = new List<JapaneseNationalHolidayData>();
    }

    public IList<JapaneseNationalHolidayData> JapaneseNationalHolidayDataList { get; }

    public async Task InitializeAsync()
    {
        if (File.Exists(JapaneseNationalHolidayDataFilePath) is false)
        {
            Assert.Fail("File not found.");
        }

        var fileStream = new FileStream(
            JapaneseNationalHolidayDataFilePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            2048,
            true);

#if NET6_0_OR_GREATER
        await using (fileStream.ConfigureAwait(false))
#else
        using (fileStream)
#endif
        {
            using var streamReader = new StreamReader(fileStream, this.shiftJisEncoding);

            var line = await streamReader.ReadLineAsync().ConfigureAwait(false);

            while (line is not null)
            {
                line = await streamReader.ReadLineAsync().ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var split = line.Split(',');

                if (split.Length != 2)
                {
                    continue;
                }

                this.JapaneseNationalHolidayDataList.Add(new JapaneseNationalHolidayData
                {
#if NET6_0_OR_GREATER
                    Date = DateOnly.Parse(split[0]),
#else
                    Date = DateTime.Parse(split[0]),
#endif
                    Name = split[1],
                });
            }
        }
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
