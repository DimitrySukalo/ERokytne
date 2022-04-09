using ERokytne.Telegram.Contracts;
using ERokytne.Telegram.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace ERokytne.Telegram;

public static class DependencyInjection
{
    public static void AddTelegramBot(this IServiceCollection services, IConfiguration configuration)
    {
        var token = configuration.GetValue<string>("Services:TelegramBot:Token");
        var url = configuration.GetValue<string>("Services:TelegramBot:Url");

        Console.WriteLine($"Token: {token}. Url: {url}");
        
        var telegramBot = new TelegramBotClient(token);
        telegramBot.SetWebhookAsync(url);
        Console.WriteLine("Webhook was setted");
        
        services.AddSingleton<ITelegramBotClient, TelegramBotClient>(_ => telegramBot);
        services.AddScoped<ITelegramBotCommandHelper, TelegramBotCommandHelper>();
    }
}