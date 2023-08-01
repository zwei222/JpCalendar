using System.Net;
using JpCalendar.Services;
using JpCalendar.WebAPI.Dto;
using Microsoft.AspNetCore.Mvc;

namespace JpCalendar.WebAPI.Controllers;

[ApiController]
public sealed class EraController : ControllerBase
{
    private readonly ILogger<EraController> logger;

    private readonly IJpCalendarService jpCalendarService;

    public EraController(
        ILogger<EraController> logger,
        IJpCalendarService jpCalendarService)
    {
        this.logger = logger;
        this.jpCalendarService = jpCalendarService;
    }

    [HttpGet]
    [Route("api/eras")]
    [ProducesResponseType(typeof(EraDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public IActionResult Get(
        [FromQuery] string date,
        [FromQuery] bool isAbbreviation,
        [FromQuery] bool isSymbol,
        [FromQuery] string format = "yy年MM月dd日")
    {
        try
        {
            if (DateOnly.TryParse(date, out var result) is false ||
                (isAbbreviation && isSymbol))
            {
                return this.BadRequest();
            }

            var era = this.jpCalendarService.GetEra(result);
            string prefix;

            if (isAbbreviation)
            {
                prefix = era.Abbreviation;
            }
            else if (isSymbol)
            {
                prefix = era.Symbol.ToString();
            }
            else
            {
                prefix = era.Name;
            }

            return this.Ok(new EraDto
            {
                Value = $"{prefix}{result.ToString(format, this.jpCalendarService.JapaneseCultureInfo)}",
                Date = result,
            });
        }
        catch (Exception exception)
        {
            this.logger.LogCritical(
                exception,
                $"{nameof(this.Get)}実行時に例外が発生しました。({nameof(date)}={date}, {nameof(isAbbreviation)}={isAbbreviation}, {nameof(isSymbol)}={isSymbol})");
            throw;
        }
    }
}
