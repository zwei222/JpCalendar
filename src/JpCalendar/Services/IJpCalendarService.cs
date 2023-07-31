using System.Globalization;

namespace JpCalendar.Services;

public interface IJpCalendarService
{
    /// <summary>
    /// Japanese CultureInfo
    /// </summary>
    CultureInfo JapaneseCultureInfo { get; }

    /// <summary>
    /// Obtains the index of the Japanese calendar to which the target date corresponds.
    /// </summary>
    /// <param name="dateTime">Date for which the Japanese calendar index is to be obtained.</param>
    /// <returns>Japanese calendar index.</returns>
    int GetEraIndex(DateTime dateTime);

    /// <summary>
    /// Obtains the Japanese calendar information corresponding to the specified Japanese calendar index.
    /// </summary>
    /// <param name="eraIndex">The index of the target Japanese calendar.</param>
    /// <returns>Japanese calendar information.</returns>
    Era GetEra(int eraIndex);

    /// <summary>
    /// Obtains Japanese calendar information for a given date.
    /// </summary>
    /// <param name="dateTime">Date for which the Japanese calendar information is to be retrieved.</param>
    /// <returns>Japanese calendar information.</returns>
    Era GetEra(DateTime dateTime);

    /// <summary>
    /// Get whether it is a leap year or not.
    /// </summary>
    /// <param name="year">Target year.</param>
    /// <returns>True is the target year is a leap year. Otherwise, it is not a leap year.</returns>
    bool IsLeapYear(int year);

    /// <summary>
    /// Gets the name of the "Rokuyo" corresponding to the specified date.
    /// </summary>
    /// <param name="dateTime">Target Date.</param>
    /// <returns>Name of "Rokuyo."</returns>
    string GetRokuyo(DateTime dateTime);

#if NET6_0_OR_GREATER
    /// <summary>
    /// Gets the name of the "Rokuyo" corresponding to the specified date.
    /// </summary>
    /// <param name="date">Target Date.</param>
    /// <returns>Name of "Rokuyo."</returns>
    string GetRokuyo(DateOnly date);
#endif

    /// <summary>
    /// Gets whether the given date is a holiday or not.
    /// </summary>
    /// <param name="dateTime">Target Date.</param>
    /// <returns>True, the date in question is a holiday. Otherwise, it is not a holiday.</returns>
    bool IsNationalHoliday(DateTime dateTime);

#if NET6_0_OR_GREATER
    /// <summary>
    /// Gets whether the given date is a holiday or not.
    /// </summary>
    /// <param name="date">Target Date.</param>
    /// <returns>True, the date in question is a holiday. Otherwise, it is not a holiday.</returns>
    bool IsNationalHoliday(DateOnly date);
#endif

    /// <summary>
    /// Gets the name of the holiday for the given date. Returns null if the given date is not a holiday.
    /// </summary>
    /// <param name="dateTime">Target Date.</param>
    /// <returns>The name of the holiday for the given date.</returns>
    string? GetNationalHolidayName(DateTime dateTime);

#if NET6_0_OR_GREATER
    /// <summary>
    /// Gets the name of the holiday for the given date. Returns null if the given date is not a holiday.
    /// </summary>
    /// <param name="date">Target Date.</param>
    /// <returns>The name of the holiday for the given date.</returns>
    string? GetNationalHolidayName(DateOnly date);
#endif
}
