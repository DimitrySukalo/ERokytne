using ERokytne.Application.Cache;
using ERokytne.Application.Telegram.Models;
using ERokytne.Domain.Constants;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands.Support.Commands;

public class SupportCommand : IRequest
{
    public string? ChatId { get; set; }
}

public class SupportCommandHandler : IRequestHandler<SupportCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserActionService _actionService;

    public SupportCommandHandler(ITelegramBotClient bot, ApplicationDbContext dbContext, UserActionService actionService)
    {
        _bot = bot;
        _dbContext = dbContext;
        _actionService = actionService;
    }

    public async Task<Unit> Handle(SupportCommand request, CancellationToken cancellationToken)
    {
        _ = await _dbContext.TelegramUsers
                .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, cancellationToken)
            ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");

        await _actionService.SetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{request.ChatId}",
            new SupportCacheModel
            {
                PreviousCommand = BotConstants.Commands.SupportCommand
            });

        await _bot.SendTextMessageAsync(request.ChatId!, 
            "Тут ви можете написати своє питання в підтримку або запропонувати свою ідею, як покращити функціонал бота 😼. " +
            "Якщо ідея буде корисна та крута, то ви отримаєте приємний бонус від нас 🔥",
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}