using Xunit;

namespace JpCalendar.Test;

public sealed class CalendarTest : IClassFixture<FileFixture>
{
    private readonly FileFixture fileFixture;

    private readonly HashSet<DateOnly> holidays;

    public CalendarTest(FileFixture fileFixture)
    {
        this.fileFixture = fileFixture;
        this.holidays = fileFixture.JapaneseNationalHolidayDataList
            .Select(holiday => holiday.Date)
            .ToHashSet();
    }

    [Fact(DisplayName = "Is holiday")]
    public void IsNationalHolidayTest001()
    {
        foreach (var holiday in this.fileFixture.JapaneseNationalHolidayDataList)
        {
            Assert.True(
                Calendar.IsNationalHoliday(holiday.Date),
                $"{holiday.Date:yyyy-MM-dd} is a national holiday. ({holiday.Name})");
            Assert.True(
                Calendar.IsNationalHoliday(holiday.Date.ToDateTime(TimeOnly.MinValue)),
                $"{holiday.Date:yyyy-MM-dd} is a national holiday. ({holiday.Name})");
            Assert.False(string.IsNullOrEmpty(Calendar.GetNationalHolidayName(holiday.Date)));
            Assert.False(string.IsNullOrEmpty(
                Calendar.GetNationalHolidayName(holiday.Date.ToDateTime(TimeOnly.MinValue))));
        }
    }

    [Fact(DisplayName = "Is not holiday")]
    public void IsNationalHolidayTest002()
    {
        for (var date = FileFixture.MinimumDate; date <= FileFixture.MaximumDate; date = date.AddDays(1))
        {
            if (this.holidays.Contains(date))
            {
                continue;
            }

            Assert.False(
                Calendar.IsNationalHoliday(date),
                $"{date:yyyy-MM-dd} is not a national holiday.");
            Assert.False(
                Calendar.IsNationalHoliday(date.ToDateTime(TimeOnly.MinValue)),
                $"{date:yyyy-MM-dd} is not a national holiday.");
            Assert.Null(Calendar.GetNationalHolidayName(date));
            Assert.Null(Calendar.GetNationalHolidayName(date.ToDateTime(TimeOnly.MinValue)));
        }
    }

    [Fact(DisplayName = "Fixture covers the checked range")]
    public void FixtureTest001()
    {
        var holidays = this.fileFixture.JapaneseNationalHolidayDataList;

        Assert.Equal(1067, holidays.Count);
        Assert.Equal(1067, this.holidays.Count);
        Assert.Equal(FileFixture.MinimumDate, holidays[0].Date);
        Assert.Equal(FileFixture.MaximumDate, holidays[^1].Date);
        Assert.All(holidays, holiday => Assert.InRange(
            holiday.Date,
            FileFixture.MinimumDate,
            FileFixture.MaximumDate));
    }
}
