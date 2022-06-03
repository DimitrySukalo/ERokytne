using System.Text;
using ERokytne.Application.Localization;
using ERokytne.Domain.Constants;
using ERokytne.Domain.Enums;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands.Subscriptions;

public class EditSubscriptionsCommand : IRequest
{
    public string? ExternalUserId { get; set; }
    
    public string? Text { get; set; }
}

public class EditSubscriptionsCommandHandler : IRequestHandler<EditSubscriptionsCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _dbContext;

    public EditSubscriptionsCommandHandler(ApplicationDbContext dbContext, ITelegramBotClient bot)
    {
        _dbContext = dbContext;
        _bot = bot;
    }

    public async Task<Unit> Handle(EditSubscriptionsCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers
                       .FirstOrDefaultAsync(e => e.ChatId == request.ExternalUserId && !e.IsRemoved, cancellationToken)
                   ?? throw new ArgumentNullException($"User with chat id {request.ExternalUserId} is not found or blocked");

        switch (request.Text)
        {
            case BotConstants.Commands.EditSubscriptions:
                await _bot.SendTextMessageAsync(request.ExternalUserId!, GetEditSubscriptionsMenu(), 
                    cancellationToken: cancellationToken);
                break;
            case BotConstants.Commands.EditWeatherSubscriptions:
                await EditDailyWeatherReportSubscription(user.Id, request.ExternalUserId);
                break;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    private static string GetEditSubscriptionsMenu()
    {
        var message = new StringBuilder();

        message.Append(
            $"{BotConstants.Commands.EditWeatherSubscriptions} - " +
            $"{Localizer.Messages.Get(BotConstants.Messages.Job.ChangeDailyWeatherReportStatus)}");

        return message.ToString();
    }

    private async Task EditDailyWeatherReportSubscription(Guid userId, string? externalUserId)
    {
        var job = await _dbContext.Jobs.Include(e => e.TelegramUser)
                      .FirstOrDefaultAsync(e => e.Type == JobType.DailyWeather && e.TelegramUserId == userId)
                  ?? throw new ArgumentNullException($"Weather job with user id {userId} is not found");

        job.IsActivated = !job.IsActivated;
        await _bot.SendTextMessageAsync(externalUserId!, Localizer.Messages
            .Get(BotConstants.Messages.Job.SubscriptionStatusChanged), cancellationToken: CancellationToken.None);
    }
}