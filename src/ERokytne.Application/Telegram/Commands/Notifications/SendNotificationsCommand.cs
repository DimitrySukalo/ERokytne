using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ERokytne.Application.Telegram.Commands.Notifications;

public class SendNotificationsCommand : IRequest
{
    public string Text { get; set; }
}

public class SendNotificationsCommandHandler : IRequestHandler<SendNotificationsCommand>
{
    private readonly ILogger<SendNotificationsCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly ITelegramBotClient _bot;

    public SendNotificationsCommandHandler(ILogger<SendNotificationsCommandHandler> logger, ApplicationDbContext dbContext,
        ITelegramBotClient bot)
    {
        _logger = logger;
        _dbContext = dbContext;
        _bot = bot;
    }

    public async Task<Unit> Handle(SendNotificationsCommand request, CancellationToken cancellationToken)
    {
        var users = await _dbContext.TelegramUsers.AsNoTracking()
            .Where(e => !e.IsRemoved).Select(e => e.ChatId).ToListAsync(cancellationToken);

        foreach (var userId in users)
        {
            try
            {
                await _bot.SendTextMessageAsync(userId!, request.Text, ParseMode.Html,
                    cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError("Error during sending message to {@userId}. Message: {@message}", 
                    users, e.Message);
            }
        }
        
        return Unit.Value;
    }
}
