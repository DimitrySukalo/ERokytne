using System.Text;
using ERokytne.Application.Localization;
using ERokytne.Domain.Constants;
using ERokytne.Domain.Enums;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ERokytne.Application.Telegram.Commands.Subscriptions;

public class GetSubscriptionsCommand : IRequest
{
    public string? ExternalUserId { get; set; }
}

public class GetSubscriptionsCommandHandler : IRequestHandler<GetSubscriptionsCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _dbContext;

    public GetSubscriptionsCommandHandler(ApplicationDbContext dbContext, ITelegramBotClient bot)
    {
        _dbContext = dbContext;
        _bot = bot;
    }

    public async Task<Unit> Handle(GetSubscriptionsCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers
                       .FirstOrDefaultAsync(e => e.ChatId == request.ExternalUserId && !e.IsRemoved, cancellationToken)
                   ?? throw new ArgumentNullException($"User with chat id {request.ExternalUserId} is not found or blocked");

        var jobs = await _dbContext.Jobs.AsNoTracking().Where(e => e.TelegramUserId == user.Id)
            .ToListAsync(cancellationToken);
        
        var message = new StringBuilder();
        
        foreach (var job in jobs)
        {
            var status = job.IsActivated
                ? Localizer.Messages.Get(BotConstants.Messages.Job.IsActivated)
                : Localizer.Messages.Get(BotConstants.Messages.Job.IsDisabled);
            
            message.Append($"{GetJobName(job.Type)} - {status}\n");
        }

        var menu = new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
        {
            new()
            {
                new InlineKeyboardButton(Localizer.Messages.Get(BotConstants.Messages.Job.EditSubscriptions))
                {
                    CallbackData = BotConstants.Commands.EditSubscriptions
                }
            }
        });
    
        await _bot.SendTextMessageAsync(request.ExternalUserId!, message.ToString(), replyMarkup: menu ,
            cancellationToken: cancellationToken);
        return Unit.Value;
    }

    private static string GetJobName(JobType jobType)
    {
        return jobType switch
        {
            JobType.DailyWeather => "Щоденна відправка погоди",
            _ => string.Empty
        };
    }
}