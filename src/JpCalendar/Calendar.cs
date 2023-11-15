using System.Globalization;
using System.Runtime.CompilerServices;
using JpCalendar.Internals;

namespace JpCalendar;

public static partial class Calendar
{
    private static readonly JapaneseCalendar JapaneseCalendar = new();

    private static readonly JapaneseLunisolarCalendar JapaneseLunisolarCalendar = new();

    private static readonly Dictionary<int, Era> EraDictionary = new();

    private static readonly Dictionary<int, string> RokuyoDictionary = new()
    {
        { 0, "大安" },
        { 1, "赤口" },
        { 2, "先勝" },
        { 3, "友引" },
        { 4, "先負" },
        { 5, "仏滅" },
    };

    static Calendar()
    {
        var dateTimeFormatInfo = JapaneseCultureInfo.DateTimeFormat;

        for (var era = 'A'; era <= 'Z'; era++)
        {
            var eraIndex = dateTimeFormatInfo.GetEra(era.ToString());

            if (eraIndex <= 0)
            {
                continue;
            }

            EraDictionary.Add(
                eraIndex,
                new Era
                {
                    Name = dateTimeFormatInfo.GetEraName(eraIndex),
                    Abbreviation = dateTimeFormatInfo.GetAbbreviatedEraName(eraIndex),
                    Symbol = era,
                });
        }

        NationalHolidayList = new NationalHolidayList(GetNationalHolidays());
    }

    public static CultureInfo JapaneseCultureInfo { get; } = new("ja-JP", false)
    {
        DateTimeFormat =
        {
            Calendar = JapaneseCalendar,
        },
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetEraIndex(DateTime dateTime)
    {
        return JapaneseCalendar.GetEra(dateTime);
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetEraIndex(DateOnly date)
    {
        return JapaneseCalendar.GetEra(date.ToDateTime(TimeOnly.MinValue));
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Era GetEra(int eraIndex)
    {
        return EraDictionary[eraIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Era GetEra(DateTime dateTime)
    {
        return GetEra(GetEraIndex(dateTime));
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Era GetEra(DateOnly date)
    {
        return GetEra(GetEraIndex(date));
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLeapYear(int year)
    {
        return JapaneseCalendar.IsLeapYear(year);
    }

    public static string GetRokuyo(DateTime dateTime)
    {
        var year = JapaneseLunisolarCalendar.GetYear(dateTime);
        var month = JapaneseLunisolarCalendar.GetMonth(dateTime);
        var day = JapaneseLunisolarCalendar.GetDayOfMonth(dateTime);
        var era = JapaneseLunisolarCalendar.GetEra(dateTime);
        var leapMonth = JapaneseLunisolarCalendar.GetLeapMonth(year, era);

        if (leapMonth > 0 && month > leapMonth)
        {
            month--;
        }

        var rokuyoIndex = (month + day) % 6;

        return RokuyoDictionary[rokuyoIndex];
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetRokuyo(DateOnly date)
    {
        return GetRokuyo(date.ToDateTime(TimeOnly.MinValue));
    }
#endif
}
