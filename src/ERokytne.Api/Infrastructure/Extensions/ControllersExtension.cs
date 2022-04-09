using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

namespace ERokytne.Api.Infrastructure.Extensions;

public static class ControllersExtension
{
    public static void ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers(configure =>
            {
                configure.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
            })
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }).AddNewtonsoftJson();
    }
    
    public static void ConfigureEndpoints(this WebApplication webApplication, string pathBase)
    {
        webApplication.UseEndpoints(endpoints =>
        {
            endpoints.MapApplicationHealthChecks();
            endpoints.MapControllers();
            endpoints.MapDefaultControllerRoute();
        });
    }
    
    private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
    {
        var builder = new ServiceCollection()
            .AddLogging()
            .AddMvc()
            .AddNewtonsoftJson()
            .Services.BuildServiceProvider();

        return builder
            .GetRequiredService<IOptions<MvcOptions>>()
            .Value
            .InputFormatters
            .OfType<NewtonsoftJsonPatchInputFormatter>()
            .First();
    }
}