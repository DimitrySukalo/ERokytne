using System.Text.Json.Serialization;

namespace ERokytne.Api.Infrastructure.Extensions;

public static class ControllersExtension
{
    public static void ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .AddNewtonsoftJson();
    }

    public static void ConfigureEndpoints(this WebApplication webApplication, string pathBase)
    {
        webApplication.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapDefaultControllerRoute();
        });
    }
}