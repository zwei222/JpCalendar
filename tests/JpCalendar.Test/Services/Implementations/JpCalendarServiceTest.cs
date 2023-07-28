using JpCalendar.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace JpCalendar.Test.Services.Implementations;

public sealed class JpCalendarServiceTest : IClassFixture<FileFixture>
{
    private readonly FileFixture fileFixture;

    private readonly IJpCalendarService jpCalendarService;

    public JpCalendarServiceTest(FileFixture fileFixture)
    {
        this.fileFixture = fileFixture;
        this.jpCalendarService = fileFixture.ServiceProvider.GetRequiredService<IJpCalendarService>();
    }

    [Fact]
    public void IsNationalHolidayTest()
    {
        foreach (var holiday in this.fileFixture.JapaneseNationalHolidayDataList)
        {
            if (this.jpCalendarService.IsNationalHoliday(holiday.Date) is false)
            {
                Assert.Fail($"{holiday.Date} is national holiday. ({holiday.Name})");
            }
        }
    }
}
