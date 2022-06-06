using ERokytne.Application.Localization;
using ERokytne.Domain.Constants;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands;

public class NotFoundCommand : IRequest<int>
{
    public long ChatId { get; set; }
}

public class NotFoundCommandHandler : IRequestHandler<NotFoundCommand, int>
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _dbContext;

    public NotFoundCommandHandler(ITelegramBotClient bot, ApplicationDbContext dbContext)
    {
        _bot = bot;
        _dbContext = dbContext;
    }

    public async Task<int> Handle(NotFoundCommand request, CancellationToken cancellationToken)
    {
        _ = await _dbContext.TelegramUsers
                .FirstOrDefaultAsync(e => e.ChatId == request.ChatId.ToString() && !e.IsRemoved, 
                    cancellationToken)
            ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");
        
        await _bot.SendTextMessageAsync(request.ChatId, 
            Localizer.Messages.Get(BotConstants.Messages.NotFound.CommandIsNotFoundMessage),
            cancellationToken: cancellationToken);
        
        return 999;
    }
}