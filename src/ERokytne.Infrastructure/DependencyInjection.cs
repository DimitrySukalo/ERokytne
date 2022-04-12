using ERokytne.Application.Ports.Adapters.Weather;
using ERokytne.Infrastructure.Adapters.WeatherApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ERokytne.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<WeatherApiOptions>(configuration.GetSection("Services:WeatherApi"));
        services.AddHttpClient<IWeatherApiAdapter, WeatherApiAdapter>();
        
        services.AddCache(configuration);
    }
        
    private static void AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEasyCaching(options =>
        {
            var connectionString = configuration.GetConnectionString("RedisConnection");

            options.WithJson(ConfigureCachingJsonSerializerSettings, "json");

            options.UseInMemory(
                config =>
                {
                    config.DBConfig.EnableWriteDeepClone = false;
                    config.DBConfig.EnableReadDeepClone = false;
                    config.EnableLogging = true;
                },
                "memory");

            options.UseRedis(
                config =>
                {
                    config.DBConfig.Configuration = connectionString;
                    config.SerializerName = "json";
                    config.EnableLogging = true;
                },
                "redis");

            options.UseHybrid(config =>
                {
                    config.TopicName = "loyalty-api-cache";
                    config.EnableLogging = true;
                    config.LocalCacheProviderName = "memory";
                    config.DistributedCacheProviderName = "redis";
                })
                .WithRedisBus(config =>
                {
                    config.Configuration = connectionString;
                    config.SerializerName = "json";
                });
        });
    }
        
    private static void ConfigureCachingJsonSerializerSettings(JsonSerializerSettings settings) =>
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
}