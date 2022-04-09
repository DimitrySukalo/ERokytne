using ERokytne.Application.Cache;
using ERokytne.Domain.Constants;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ERokytne.Application.Telegram.Commands.Announcements;

public class CancelAnnouncementCommand : IRequest
{
    public string? ChatId { get; set; }
    
    public Guid? Id { get; set; }
}

public class CancelAnnouncementCommandHandler : IRequestHandler<CancelAnnouncementCommand>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserActionService _actionService;
    private readonly ITelegramBotClient _client;

    public CancelAnnouncementCommandHandler(ApplicationDbContext dbContext, UserActionService actionService,
        ITelegramBotClient client)
    {
        _dbContext = dbContext;
        _actionService = actionService;
        _client = client;
    }

    public async Task<Unit> Handle(CancelAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers
                       .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, cancellationToken)
                   ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");

        var announcement =
            await _dbContext.Announcements.FirstOrDefaultAsync(e => 
                    e.Id == request.Id && e.TelegramUserId == user.Id,
                cancellationToken) ?? 
            throw new ArgumentNullException($"Announcement with id {request.Id} is not found");

        _dbContext.Announcements.Remove(announcement);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _actionService.DeleteUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{request.ChatId}");
        
        var menu = new ReplyKeyboardMarkup(new List<KeyboardButton>
        {
            new(BotConstants.Commands.SellCommand)
        })
        {
            ResizeKeyboard = true
        };
            
        await _client.SendTextMessageAsync(request.ChatId!, "Оголошення успішно відмінено!"
            ,replyMarkup: menu, cancellationToken: cancellationToken);
        
        return Unit.Value;
    }
}