using System.Globalization;
using System.Runtime.CompilerServices;
using JpCalendar.Internals;

namespace JpCalendar.Services.Implementations;

internal sealed partial class JpCalendarService : IJpCalendarService
{
    private readonly JapaneseCalendar japaneseCalendar;

    private readonly JapaneseLunisolarCalendar japaneseLunisolarCalendar;

    private readonly Dictionary<int, Era> eraDictionary;

    private readonly Dictionary<int, string> rokuyoDictionary;

    public JpCalendarService()
    {
        this.japaneseCalendar = new JapaneseCalendar();
        this.japaneseLunisolarCalendar = new JapaneseLunisolarCalendar();
        this.JapaneseCultureInfo = new CultureInfo("ja-JP", false)
        {
            DateTimeFormat =
            {
                Calendar = this.japaneseCalendar,
            },
        };
        this.eraDictionary = new Dictionary<int, Era>();
        this.rokuyoDictionary = new Dictionary<int, string>
        {
            { 0, "大安" },
            { 1, "赤口" },
            { 2, "先勝" },
            { 3, "友引" },
            { 4, "先負" },
            { 5, "仏滅" },
        };

        var dateTimeFormatInfo = this.JapaneseCultureInfo.DateTimeFormat;

        for (var era = 'A'; era <= 'Z'; era++)
        {
            var eraIndex = dateTimeFormatInfo.GetEra(era.ToString());

            if (eraIndex <= 0)
            {
                continue;
            }

            this.eraDictionary.Add(
                eraIndex,
                new Era
                {
                    Name = dateTimeFormatInfo.GetEraName(eraIndex),
                    Abbreviation = dateTimeFormatInfo.GetAbbreviatedEraName(eraIndex),
                    Symbol = era,
                });
        }

        this.nationalHolidayList = new NationalHolidayList(this.GetNationalHolidays());
        this.InitializeNationalHolidays();
    }

    public CultureInfo JapaneseCultureInfo { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetEraIndex(DateTime dateTime)
    {
        return this.japaneseCalendar.GetEra(dateTime);
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetEraIndex(DateOnly date)
    {
        return this.japaneseCalendar.GetEra(date.ToDateTime(TimeOnly.MinValue));
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Era GetEra(int eraIndex)
    {
        return this.eraDictionary[eraIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Era GetEra(DateTime dateTime)
    {
        return this.GetEra(this.GetEraIndex(dateTime));
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Era GetEra(DateOnly date)
    {
        return this.GetEra(this.GetEraIndex(date));
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLeapYear(int year)
    {
        return this.japaneseCalendar.IsLeapYear(year);
    }

    public string GetRokuyo(DateTime dateTime)
    {
        var year = this.japaneseLunisolarCalendar.GetYear(dateTime);
        var month = this.japaneseLunisolarCalendar.GetMonth(dateTime);
        var day = this.japaneseLunisolarCalendar.GetDayOfMonth(dateTime);
        var era = this.japaneseLunisolarCalendar.GetEra(dateTime);
        var leapMonth = this.japaneseLunisolarCalendar.GetLeapMonth(year, era);

        if (leapMonth > 0 && month > leapMonth)
        {
            month--;
        }

        var rokuyoIndex = (month + day) % 6;

        return this.rokuyoDictionary[rokuyoIndex];
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetRokuyo(DateOnly date)
    {
        return this.GetRokuyo(date.ToDateTime(TimeOnly.MinValue));
    }
#endif
}
