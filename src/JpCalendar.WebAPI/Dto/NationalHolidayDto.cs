﻿namespace JpCalendar.WebAPI.Dto;

public sealed class NationalHolidayDto
{
    public string? Value { get; set; } = string.Empty;

    public DateOnly? Date { get; set; }
}
