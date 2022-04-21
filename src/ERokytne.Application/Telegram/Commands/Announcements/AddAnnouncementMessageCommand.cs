using ERokytne.Application.Cache;
using ERokytne.Application.Localization;
using ERokytne.Application.Telegram.Models;
using ERokytne.Domain.Constants;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ERokytne.Application.Telegram.Commands.Announcements;

public class AddAnnouncementMessageCommand : IRequest
{
    public string? ChatId { get; set; }
    
    public Guid? AnnouncementId { get; set; }
    
    public string? Text { get; set; }
}

public class AddAnnouncementMessageCommandHandler : IRequestHandler<AddAnnouncementMessageCommand>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserActionService _actionService;
    private readonly ITelegramBotClient _client;

    public AddAnnouncementMessageCommandHandler(ApplicationDbContext dbContext, UserActionService actionService, 
        ITelegramBotClient client)
    {
        _dbContext = dbContext;
        _actionService = actionService;
        _client = client;
    }

    public async Task<Unit> Handle(AddAnnouncementMessageCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers
                       .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, cancellationToken)
                   ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");

        if (request.Text?.Length > 5000)
        {
            await _client.SendTextMessageAsync(request.ChatId!,
                Localizer.Messages.Get(BotConstants.Messages.Announcement.LimitTextLengthMessage),
                cancellationToken: cancellationToken);
            return Unit.Value;
        }

        var announcement =
            await _dbContext.Announcements.FirstOrDefaultAsync(e => 
                    e.Id == request.AnnouncementId && e.TelegramUserId == user.Id,
                cancellationToken) ?? 
            throw new ArgumentNullException($"Announcement with id {request.AnnouncementId} is not found");

        announcement.Text = request.Text;
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        await _actionService.SetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{request.ChatId}",
            new CacheModel
            {
                PreviousCommand = BotConstants.Commands.AnnouncementEnteredText,
                Announcement = new AnnouncementCache
                {
                    Id = announcement.Id
                }
            });
        
        var menu = new ReplyKeyboardMarkup(new List<KeyboardButton>
        {
            new(BotConstants.Commands.PostAnnouncement),
            new(BotConstants.Commands.CancelAnnouncement),
        })
        {
            ResizeKeyboard = true
        };

        await _client.SendTextMessageAsync(request.ChatId!, 
            Localizer.Messages.Get(BotConstants.Messages.Announcement.TextIsSavedMessage), 
            replyMarkup: menu, cancellationToken: cancellationToken);
        
        return Unit.Value;
    }
}