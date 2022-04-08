using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ERokytne.Persistence;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAccessTokenManagement(options =>
        {
            options.Client.Clients.Add("identity-client", new ClientCredentialsTokenRequest
            {
                Address = $"{configuration.GetValue<string>("Services:Identity:Url").TrimEnd('/')}/connect/token",
                ClientId = configuration.GetValue<string>("Services:Identity:ClientId"),
                ClientSecret = configuration.GetValue<string>("Services:Identity:ClientSecret"),
                Scope = configuration.GetValue<string>("Services:Identity:DefaultScope"),
            });
        });

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