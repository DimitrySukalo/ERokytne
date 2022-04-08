using ERokytne.Api.Services;
using ERokytne.Application.Telegram.Commands;
using ERokytne.Persistence;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ERokytne.Api.Infrastructure.Extensions;

public static class ServicesExtension
{
    public static void AddDiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });
        
        services.AddSingleton<FileExtensionContentTypeProvider>();
        services.AddHttpContextAccessor();
        services.AddMediatR(typeof(NotFoundCommand).Assembly);
        services.AddTransient<IEmailSender, FakeEmailSender>();
    }

    public static async Task InitDatabase(IConfiguration configuration, WebApplication webApplication)
    {
        if (!configuration.GetValue<bool>("General:AutoMigrations"))
        {
            return;
        }

        try
        { 
            using var scope = webApplication.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while migrating or initializing the database");
        }
    }
}