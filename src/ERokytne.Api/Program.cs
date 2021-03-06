using ERokytne.Api.Filters;
using ERokytne.Api.Helpers;
using ERokytne.Api.Infrastructure.Extensions;
using ERokytne.Application.Localization;
using ERokytne.Domain.Entities;
using ERokytne.Infrastructure;
using ERokytne.Persistence;
using ERokytne.Telegram;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Json;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json",false, true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",false, true)
    .AddEnvironmentVariables()
    .Build();

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseConfiguration(configuration);

var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.File(
        new JsonFormatter(renderMessage: true),
        "./App_Logs/log.json",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true,
        fileSizeLimitBytes: 52_428_800,
        flushToDiskInterval: TimeSpan.FromSeconds(1),
        shared: true)
    .WriteTo.Console();

// if (!builder.Environment.IsDevelopment())
// {
//     loggerConfiguration
//         .WriteTo.GoogleCloudLogging(new GoogleCloudLoggingSinkOptions
//         {
//             ProjectId = "ERokytne", UseJsonOutput = true, ServiceName = "erokytne-bot"
//         });
// }

Log.Logger = loggerConfiguration
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetConnectionString("DefaultDbConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.ConfigureControllers();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureControllers();
builder.Services.AddApplicationHealthChecks(configuration);
builder.Services.AddInfrastructure(configuration);
builder.Services.AddTelegramBot(configuration);
builder.Services.AddDiServices(configuration);
builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();


builder.Services.AddDefaultIdentity<Admin>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
var mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

await Localizer.LoadFromAssembliesAsync(typeof(Program).Assembly);
try
{
    var app = builder.Build();
    await ServicesExtension.InitDatabase(configuration, app);
    app.UseStaticFiles();
    app.UseRouting();
    app.SetInfrastructureOptions(configuration);
    app.UseSwagger(o => o.SerializeAsV2 = false);
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new [] { new MyAuthorizationFilter() }
    });
    app.ConfigureEndpoints();
    app.MapRazorPages();
    
    //Scheduling jobs
    await ScheduleCommandsHelper.ScheduleSendReportAsync(app);
    
    //run app
    app.Run();
}
catch (Exception e)
{
    Log.Logger.Fatal(e, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}