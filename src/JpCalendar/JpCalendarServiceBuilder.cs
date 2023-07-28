using JpCalendar.Services;
using JpCalendar.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace JpCalendar;

public sealed class JpCalendarServiceBuilder
{
    public JpCalendarServiceBuilder(IServiceCollection services)
    {
        this.Services = services;
        services.AddSingleton<IJpCalendarService, JpCalendarService>();
    }

    public IServiceCollection Services { get; }
}
