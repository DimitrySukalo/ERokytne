using ERokytne.Application.Cache;
using ERokytne.Domain.Constants;
using ERokytne.Domain.Entities;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands.Support.Commands;

public class SaveSupportMessageCommand : IRequest
{
    public string? ChatId { get; set; }
    
    public string? Text { get; set; }
}

public class SaveSupportMessageCommandHandler : IRequestHandler<SaveSupportMessageCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserActionService _actionService;

    public SaveSupportMessageCommandHandler(ITelegramBotClient bot, ApplicationDbContext dbContext,
        UserActionService actionService)
    {
        _bot = bot;
        _dbContext = dbContext;
        _actionService = actionService;
    }

    public async Task<Unit> Handle(SaveSupportMessageCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers
                .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, cancellationToken)
            ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");

        if (request.Text?.Length > 5000)
        {
            await _bot.SendTextMessageAsync(request.ChatId!,
                "Довжина вашого тексту перевищує встановлений ліміт ( 5000 символів ) ☝️",
                cancellationToken: cancellationToken);
            return Unit.Value;
        }
        
        user.SupportMessages.Add(new SupportMessage
        {
            Text = request.Text
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _actionService.DeleteUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{request.ChatId}");

        await _bot.SendTextMessageAsync(request.ChatId!, "Дякуємо! Ваше повідомлення збережено 👌",
            cancellationToken: cancellationToken);
        
        return Unit.Value;
    }
}