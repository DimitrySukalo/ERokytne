using System.ComponentModel.DataAnnotations;
using ERokytne.Application.Helpers;
using ERokytne.Application.Localization;
using ERokytne.Application.Ports.Adapters.Weather;
using ERokytne.Domain.Constants;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ERokytne.Application.Telegram.Commands.Weather;

public class SendWeatherCommand : IRequest
{
    public List<string?> Chats { get; set; }
}

public class SendWeatherCommandHandler : IRequestHandler<SendWeatherCommand>
{
    private readonly IWeatherApiAdapter _weatherApiAdapter;
    private readonly ITelegramBotClient _botClient;

    public SendWeatherCommandHandler(IWeatherApiAdapter weatherApiAdapter, ITelegramBotClient botClient)
    {
        _weatherApiAdapter = weatherApiAdapter;
        _botClient = botClient;
    }

    public async Task<Unit> Handle(SendWeatherCommand request, CancellationToken cancellationToken)
    {
        var weather = await _weatherApiAdapter.GetWeatherInfo(1, false, false);
        if (weather!.Forecast.ForecastDays.Any())
        {
            foreach (var chatId in request.Chats)
            {
                await _botClient.SendTextMessageAsync(chatId!,
                    Localizer.Messages.Get(BotConstants.Messages.NewDay.HelloMessage), 
                    cancellationToken: cancellationToken);
                
                await _botClient.SendTextMessageAsync(chatId!,
                    MessageHelper.GetWeatherMessage(weather.Forecast.ForecastDays.FirstOrDefault()!,
                        DateTime.UtcNow.ToString("yyyy-MM-dd")), ParseMode.Html, 
                    cancellationToken: cancellationToken);
            }
        }
        else
        {
            throw new ValidationException("Weather response is empty");
        }
        
        return Unit.Value;
    }
}
