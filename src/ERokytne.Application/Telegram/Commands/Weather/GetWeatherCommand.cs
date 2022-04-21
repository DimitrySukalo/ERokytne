using ERokytne.Application.Cache;
using ERokytne.Application.Localization;
using ERokytne.Application.Telegram.Models;
using ERokytne.Domain.Constants;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ERokytne.Application.Telegram.Commands.Weather;

public class GetWeatherCommand : IRequest
{
    public string ChatId { get; set; }
    
    public int? MessageId { get; set; }
}

public class GetWeatherCommandHandler : IRequestHandler<GetWeatherCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserActionService _userAction;

    public GetWeatherCommandHandler(ITelegramBotClient bot, ApplicationDbContext dbContext, UserActionService userAction)
    {
        _bot = bot;
        _dbContext = dbContext;
        _userAction = userAction;
    }

    public async Task<Unit> Handle(GetWeatherCommand request, CancellationToken cancellationToken)
    {
        _ = await _dbContext.TelegramUsers
                .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, 
                    cancellationToken)
            ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");

        var date = DateTime.UtcNow;
        
        var firstDay = date.ToString("yyyy-MM-dd");
        var secondDay = date.AddDays(1).ToString("yyyy-MM-dd");
        var thirdDay = date.AddDays(2).ToString("yyyy-MM-dd");
        
        var menu = new InlineKeyboardMarkup(new List<IEnumerable<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                new(firstDay)
                {
                    CallbackData = firstDay
                }
            },
            new List<InlineKeyboardButton>
            {
                new(secondDay)
                {
                    CallbackData = secondDay
                }
            },
            new List<InlineKeyboardButton>
            {
                new(thirdDay)
                {
                    CallbackData = thirdDay
                }
            }
        });

        int messageId;
        if (request.MessageId.HasValue)
        {
            await _bot.EditMessageTextAsync(request.ChatId, request.MessageId.Value,
                Localizer.Messages.Get(BotConstants.Messages.Weather.SelectDayMessage),
                replyMarkup: menu, cancellationToken: cancellationToken);

            messageId = request.MessageId.Value;
        }
        else
        {
            var message = await _bot.SendTextMessageAsync(request.ChatId, 
                Localizer.Messages.Get(BotConstants.Messages.Weather.SelectDayMessage),
                replyMarkup: menu, cancellationToken: cancellationToken);

            messageId = message.MessageId;
        }

        await _userAction.SetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{request.ChatId}", new CacheModel
        {
            PreviousCommand = BotConstants.Commands.WeatherCommand,
            Weather = new WeatherCache
            {
                MessageId = messageId
            }
        });

        return Unit.Value;
    }
}