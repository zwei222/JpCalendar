﻿namespace JpCalendar.Internals;

internal
#if NET6_0_OR_GREATER
    readonly
#endif
    struct NationalHoliday
{
    public
#if NET7_0_OR_GREATER
        required
#endif
        bool IsFixedDay
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    public
#if NET7_0_OR_GREATER
        required
#endif
        bool IsFixedWeek
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    public
#if NET7_0_OR_GREATER
        required
#endif
        int TransferPeriod
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    public
#if NET7_0_OR_GREATER
        required
#endif
        string Name
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    public
#if NET7_0_OR_GREATER
        required
#endif
        int Month
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    public
#if NET7_0_OR_GREATER
        required
#endif
        int Day
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    public
#if NET7_0_OR_GREATER
        required
#endif
        int Week
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    public
#if NET7_0_OR_GREATER
        required
#endif
        int DayOfWeek
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    public
#if NET7_0_OR_GREATER
        required
#endif
#if NET6_0_OR_GREATER
        DateOnly?
#else
        DateTime?
#endif
        StartDate
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    public
#if NET7_0_OR_GREATER
        required
#endif
#if NET6_0_OR_GREATER
        DateOnly?
#else
        DateTime?
#endif
        EndDate
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    public
#if NET7_0_OR_GREATER
        required
#endif
#if NET6_0_OR_GREATER
        DateOnly[]?
#else
        DateTime[]?
#endif
        ExceptionDate
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    public override int GetHashCode()
    {
#if NET6_0_OR_GREATER
        return HashCode.Combine(
            this.IsFixedDay,
            this.IsFixedWeek,
            this.TransferPeriod,
            this.Name,
            this.Month,
            this.Day,
            this.Week,
            this.DayOfWeek);
#else
        return (
            this.IsFixedDay,
            this.IsFixedWeek,
            this.TransferPeriod,
            this.Name,
            this.Month,
            this.Day,
            this.Week,
            this.DayOfWeek).GetHashCode();
#endif
    }
}
