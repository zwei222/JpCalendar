namespace JpCalendar.Test;

public sealed class JapaneseNationalHolidayData
{
    public
#if NET6_0_OR_GREATER
        DateOnly
#else
        DateTime
#endif
        Date
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    public string Name
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    } = string.Empty;
}
