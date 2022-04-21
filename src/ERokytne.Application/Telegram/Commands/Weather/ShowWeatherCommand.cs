using ERokytne.Application.Cache;
using ERokytne.Application.Helpers;
using ERokytne.Application.Localization;
using ERokytne.Application.Ports.Adapters.Weather;
using ERokytne.Application.Telegram.Models;
using ERokytne.Domain.Constants;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ERokytne.Application.Telegram.Commands.Weather;

public class ShowWeatherCommand : IRequest
{
    public string ChatId { get; set; }
    
    public string? Day { get; set; }
    
    public int MessageId { get; set; }
}

public class ShowWeatherCommandHandler : IRequestHandler<ShowWeatherCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _dbContext;
    private readonly IWeatherApiAdapter _weatherApi;
    private readonly UserActionService _userAction;

    public ShowWeatherCommandHandler(ITelegramBotClient bot, ApplicationDbContext dbContext,
        IWeatherApiAdapter weatherApi, UserActionService userAction)
    {
        _bot = bot;
        _dbContext = dbContext;
        _weatherApi = weatherApi;
        _userAction = userAction;
    }

    public async Task<Unit> Handle(ShowWeatherCommand request, CancellationToken cancellationToken)
    {
        _ = await _dbContext.TelegramUsers
                .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, 
                    cancellationToken)
            ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");

        var weatherInfo = await _weatherApi.GetWeatherInfo(3, false, false);
        var selectedDayInfo = weatherInfo?.Forecast.ForecastDays
            .FirstOrDefault(e => e.Date.Equals(request.Day));

        var menu = new InlineKeyboardMarkup(new List<IEnumerable<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                new(Localizer.Messages.Get(BotConstants.Messages.Weather.BackToDayListMessage))
                {
                    CallbackData = BotConstants.Commands.ReturnWeatherDayList
                }
            }
        });

        var cache = await _userAction
            .GetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{request.ChatId}",
                () => Task.FromResult(new CacheModel()));

        cache.PreviousCommand = BotConstants.Commands.WeatherIsSelected;
        await _userAction.SetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{request.ChatId}", cache);
        
        if (selectedDayInfo is null)
        {
            await _bot.EditMessageTextAsync(request.ChatId, request.MessageId,
                Localizer.Messages.Get(BotConstants.Messages.Weather.DataNotFoundMessage), 
                replyMarkup: menu, cancellationToken: cancellationToken);
        }
        else
        {
            await _bot.EditMessageTextAsync(request.ChatId, request.MessageId,
                MessageHelper.GetWeatherMessage(selectedDayInfo, request.Day), replyMarkup: menu, 
                 parseMode: ParseMode.Html, cancellationToken: cancellationToken);
        }
        
        return Unit.Value;
    }
}