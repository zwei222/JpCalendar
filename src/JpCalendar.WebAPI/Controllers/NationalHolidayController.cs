using System.Net;
using JpCalendar.Services;
using JpCalendar.WebAPI.Dto;
using Microsoft.AspNetCore.Mvc;

namespace JpCalendar.WebAPI.Controllers;

[ApiController]
public sealed class NationalHolidayController : ControllerBase
{
    private readonly ILogger<NationalHolidayController> logger;

    private readonly IJpCalendarService jpCalendarService;

    public NationalHolidayController(
        ILogger<NationalHolidayController> logger,
        IJpCalendarService jpCalendarService)
    {
        this.logger = logger;
        this.jpCalendarService = jpCalendarService;
    }

    [HttpGet]
    [Route("api/national-holidays")]
    [ProducesResponseType(typeof(IEnumerable<NationalHolidayDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public IActionResult Get([FromQuery] string from, [FromQuery] string to)
    {
        try
        {
            if (DateOnly.TryParse(from, out var fromDate) is false ||
                DateOnly.TryParse(to, out var toDate) is false ||
                fromDate > toDate)
            {
                return this.BadRequest();
            }

            var results = new List<NationalHolidayDto>();
            var currentDate = fromDate;

            while (currentDate <= toDate)
            {
                var name = this.jpCalendarService.GetNationalHolidayName(currentDate);

                if (name is null)
                {
                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                var nationalHoliday = new NationalHolidayDto
                {
                    Name = name,
                    Date = currentDate,
                };

                results.Add(nationalHoliday);
                currentDate = currentDate.AddDays(1);
            }

            if (results.Any() is false)
            {
                return this.NotFound();
            }

            return this.Ok(results);
        }
        catch (Exception exception)
        {
            this.logger.LogCritical(
                exception,
                $"{nameof(this.Get)}実行時に例外が発生しました。({nameof(from)}={from}, {nameof(to)}={to})");
            throw;
        }
    }

    [HttpGet]
    [Route("api/national-holidays/{date}")]
    [ProducesResponseType(typeof(NationalHolidayDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public IActionResult Get(string date)
    {
        try
        {
            if (DateOnly.TryParse(date, out var result) is false)
            {
                return this.BadRequest();
            }

            var nationalHoliday = new NationalHolidayDto
            {
                Name = this.jpCalendarService.GetNationalHolidayName(result), Date = result,
            };

            if (nationalHoliday.Name is null)
            {
                return this.NotFound();
            }

            return this.Ok(nationalHoliday);
        }
        catch (Exception exception)
        {
            this.logger.LogCritical(
                exception,
                $"{nameof(this.Get)}実行時に例外が発生しました。({nameof(date)}={date})");
            throw;
        }
    }
}
