using Xunit;

namespace JpCalendar.Test;

public sealed class CalendarTest : IClassFixture<FileFixture>
{
    private readonly FileFixture fileFixture;

    public CalendarTest(FileFixture fileFixture)
    {
        this.fileFixture = fileFixture;
    }

    [Fact(DisplayName = "Is holiday")]
    public void IsNationalHolidayTest001()
    {
        foreach (var holiday in this.fileFixture.JapaneseNationalHolidayDataList)
        {
            // for debug
            if (new DateTime(2020, 7, 24) == new DateTime(holiday.Date.Year, holiday.Date.Month, holiday.Date.Day))
            {
                ;
            }

            if (Calendar.IsNationalHoliday(holiday.Date) is false)
            {
                Assert.Fail($"{holiday.Date} is national holiday. ({holiday.Name})");
            }
        }
    }

    [Fact(DisplayName = "Is not holiday")]
    public void IsNationalHolidayTest002()
    {
        var currentDate = new DateTime(1955, 1, 1);

        foreach (var holiday in this.fileFixture.JapaneseNationalHolidayDataList)
        {
            while (true)
            {
                // for debug
                if (new DateTime(2021, 7, 24) == currentDate)
                {
                    ;
                }

                if (currentDate == new DateTime(holiday.Date.Year, holiday.Date.Month, holiday.Date.Day))
                {
                    currentDate = currentDate.AddDays(1);
                    break;
                }

                if (Calendar.IsNationalHoliday(currentDate))
                {
                    Assert.Fail($"{currentDate} is not national holiday.");
                }

                currentDate = currentDate.AddDays(1);
            }
        }
    }
}
