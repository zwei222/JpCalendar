using System.Globalization;
using JpCalendar.Internals;

namespace JpCalendar.Services;

public interface IJpCalendarService
{
    CultureInfo JapaneseCultureInfo { get; }

    int GetEraIndex(DateTime dateTime);

    Era GetEra(int eraIndex);

    Era GetEra(DateTime dateTime);

    bool IsLeapYear(int year);

    string GetRokuyo(DateTime dateTime);

    string GetRokuyo(DateOnly dateOnly);

    bool IsNationalHoliday(DateTime dateTime);

    bool IsNationalHoliday(DateOnly date);

    string? GetNationalHolidayName(DateTime dateTime);

    string? GetNationalHolidayName(DateOnly date);
}
