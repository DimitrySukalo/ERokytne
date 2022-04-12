using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands.Notifications;

public class SendNotificationCommand : IRequest
{
    public long ChatId { get; set; }
    
    public string Text { get; set; }
}

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _dbContext;
    
    public SendNotificationCommandHandler(ITelegramBotClient bot, ApplicationDbContext dbContext)
    {
        _bot = bot;
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        _ = await _dbContext.TelegramUsers.AsNoTracking()
                .FirstOrDefaultAsync(e => e.ChatId == request.ChatId.ToString(), cancellationToken)
            ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not exist");

        await _bot.SendTextMessageAsync(request.ChatId, request.Text, cancellationToken: cancellationToken);
        return Unit.Value;
    }
}
