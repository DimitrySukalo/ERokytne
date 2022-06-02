using ERokytne.Application.Scheduler.Models;
using ERokytne.Application.Telegram.Commands.Weather;
using MassTransit;
using IHost = Microsoft.Extensions.Hosting.IHost;

namespace ERokytne.Api.Helpers;

public static class ScheduleCommandsHelper
{
    private static readonly Uri SchedulerQueue = new("queue:hangfire");
    private static readonly Uri JobsQueue = new("queue:ERokytne:Jobs");
    
    public static async Task ScheduleSendReportAsync(IHost webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var services = scope.ServiceProvider;

        var sendEndpointProvider = services.GetRequiredService<ISendEndpointProvider>();
                
        var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(SchedulerQueue);
        await sendEndpoint.ScheduleRecurringSend(JobsQueue,
            new ServiceRecurringSchedule("SendDailyWeather", "Bot", "0 8 * * *"), 
            new UpdateWeatherCommand(), CancellationToken.None);
    }
}