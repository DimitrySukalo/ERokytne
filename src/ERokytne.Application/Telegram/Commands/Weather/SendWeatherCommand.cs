using System.ComponentModel.DataAnnotations;
using ERokytne.Application.Helpers;
using ERokytne.Application.Ports.Adapters.Weather;
using MediatR;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands.Weather;

public class SendWeatherCommand : IRequest
{
    public List<string> Chats { get; set; }
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
                await _botClient.SendTextMessageAsync(chatId,
                    MessageHelper.GetWeatherMessage(weather.Forecast.ForecastDays.FirstOrDefault()!,
                        DateTime.UtcNow.ToString("yyyy-MM-dd")), cancellationToken: cancellationToken);
            }
        }
        else
        {
            throw new ValidationException("Weather response is empty");
        }
        
        return Unit.Value;
    }
}
