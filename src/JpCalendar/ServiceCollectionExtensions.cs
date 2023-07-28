using Microsoft.Extensions.DependencyInjection;

namespace JpCalendar;

public static class ServiceCollectionExtensions
{
    public static JpCalendarServiceBuilder AddJpCalendar(this IServiceCollection services)
    {
        return new JpCalendarServiceBuilder(services);
    }
}
