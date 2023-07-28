namespace JpCalendar.WebAPI.Dto;

public sealed class NationalHolidayDto
{
    public string? Name { get; set; } = string.Empty;

    public DateOnly? Date { get; set; }
}
