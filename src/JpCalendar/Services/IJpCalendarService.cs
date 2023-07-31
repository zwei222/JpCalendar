using System.Globalization;

namespace JpCalendar.Services;

public interface IJpCalendarService
{
    CultureInfo JapaneseCultureInfo { get; }

    int GetEraIndex(DateTime dateTime);

    Era GetEra(int eraIndex);

    Era GetEra(DateTime dateTime);

    bool IsLeapYear(int year);

    string GetRokuyo(DateTime dateTime);

#if NET6_0_OR_GREATER
    string GetRokuyo(DateOnly dateOnly);
#endif

    bool IsNationalHoliday(DateTime dateTime);

#if NET6_0_OR_GREATER
    bool IsNationalHoliday(DateOnly date);
#endif

    string? GetNationalHolidayName(DateTime dateTime);

#if NET6_0_OR_GREATER
    string? GetNationalHolidayName(DateOnly date);
#endif
}
