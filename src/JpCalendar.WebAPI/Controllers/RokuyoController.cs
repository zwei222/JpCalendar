using System.Net;
using JpCalendar.WebAPI.Dto;
using Microsoft.AspNetCore.Mvc;

namespace JpCalendar.WebAPI.Controllers;

[ApiController]
public class RokuyoController : ControllerBase
{
    private readonly ILogger<RokuyoController> logger;

    public RokuyoController(ILogger<RokuyoController> logger)
    {
        this.logger = logger;
    }

    [HttpGet]
    [Route("api/rokuyo")]
    [ProducesResponseType(typeof(RokuyoDto[]), (int)HttpStatusCode.OK)]
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

            var results = new List<RokuyoDto>();
            var currentDate = fromDate;

            while (currentDate <= toDate)
            {
                var name = Calendar.GetRokuyo(currentDate);

                var rokuyo = new RokuyoDto()
                {
                    Value = name,
                    Date = currentDate,
                };

                results.Add(rokuyo);
                currentDate = currentDate.AddDays(1);
            }

            if (results.Any() is false)
            {
                return this.NotFound();
            }

            return this.Ok(results.ToArray());
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
    [Route("api/rokuyo/{date}")]
    [ProducesResponseType(typeof(RokuyoDto), (int)HttpStatusCode.OK)]
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

            var rokuyo = new RokuyoDto
            {
                Value = Calendar.GetRokuyo(result),
                Date = result,
            };

            if (rokuyo.Value is null)
            {
                return this.NotFound();
            }

            return this.Ok(rokuyo);
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
