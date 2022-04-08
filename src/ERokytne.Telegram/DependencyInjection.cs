using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace ERokytne.Telegram;

public static class DependencyInjection
{
    public static void AddTelegramBot(this IServiceCollection services, IConfiguration configuration)
    {
        var telegramBot = new TelegramBotClient(configuration.GetValue<string>("Services:TelegramBot:Token"));
        telegramBot.SetWebhookAsync(configuration.GetValue<string>("Services:TelegramBot:Url"));
        
        services.AddSingleton<ITelegramBotClient, TelegramBotClient>(_ => telegramBot);
    }
}