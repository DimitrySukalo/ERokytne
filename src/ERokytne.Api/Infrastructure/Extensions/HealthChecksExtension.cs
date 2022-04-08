using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace ERokytne.Api.Infrastructure.Extensions;

public static class HealthChecksExtension
{
    private const string Liveness = "live";
    private const string Readiness = "ready";
        
    public static void AddApplicationHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddSqlServer(
                configuration.GetConnectionString("DefaultDbConnection"),
                tags: new[] {Liveness, Readiness})
            .AddRedis(
                configuration.GetConnectionString("RedisConnection"),
                tags: new[] {Liveness, Readiness});
    }

    public static void MapApplicationHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        endpoints.MapHealthChecks("/health/_self", new HealthCheckOptions
        {
            Predicate = _ => false,
        });

        endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(Liveness)
        });

        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(Readiness)
        });
    }
}