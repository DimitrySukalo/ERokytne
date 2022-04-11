using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands;

public class NotFoundCommand : IRequest
{
    public long ChatId { get; set; }
}

public class NotFoundCommandHandler : IRequestHandler<NotFoundCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _dbContext;

    public NotFoundCommandHandler(ITelegramBotClient bot, ApplicationDbContext dbContext)
    {
        _bot = bot;
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(NotFoundCommand request, CancellationToken cancellationToken)
    {
        _ = await _dbContext.TelegramUsers
                .FirstOrDefaultAsync(e => e.ChatId == request.ChatId.ToString() && !e.IsRemoved, 
                    cancellationToken)
            ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");
        
        await _bot.SendTextMessageAsync(request.ChatId, "Команда не найдена.", cancellationToken: cancellationToken);
        return Unit.Value;
    }
}