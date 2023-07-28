namespace JpCalendar.Internals;

internal readonly struct NationalHoliday
{
    public required bool IsFixedDay { get; init; }

    public required bool IsFixedWeek { get; init; }

    public required int TransferPeriod { get; init; }

    public required string Name { get; init; }

    public required int Month { get; init; }

    public required int Day { get; init; }

    public required int Week { get; init; }

    public required int DayOfWeek { get; init; }

    public required DateOnly? StartDate { get; init; }

    public required DateOnly? EndDate { get; init; }

    public required DateOnly[]? ExceptionDate { get; init; }
}
