using ERokytne.Application.Cache;
using ERokytne.Application.Localization;
using ERokytne.Application.Telegram.Models;
using ERokytne.Domain.Constants;
using ERokytne.Domain.Entities;
using ERokytne.Domain.Enums;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ERokytne.Application.Telegram.Commands.Announcements;

public class CreateAnnouncement : IRequest
{
    public string? ChatId { get; set; }
}

public class CreateAnnouncementHandler : IRequestHandler<CreateAnnouncement>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserActionService _actionService;
    private readonly ITelegramBotClient _client;

    public CreateAnnouncementHandler(ApplicationDbContext dbContext, UserActionService actionService, ITelegramBotClient client)
    {
        _dbContext = dbContext;
        _actionService = actionService;
        _client = client;
    }

    public async Task<Unit> Handle(CreateAnnouncement request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers.Include(e => e.Announcements)
            .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, cancellationToken)
                   ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");

        if (user.Announcements.Count(e => e.CreatedOn.Date == DateTime.UtcNow.Date && !e.IsRemoved) >= 3 && 
            user.Type == TelegramUserType.User)
        {
            await _client.SendTextMessageAsync(request.ChatId!,
                Localizer.Messages.Get(BotConstants.Messages.Announcement.LimitMessage),
                cancellationToken: cancellationToken);
            
            return Unit.Value;
        }

        if (string.IsNullOrWhiteSpace(user.NickName) || user.NickName.Equals("@"))
        {
            await _client.SendTextMessageAsync(request.ChatId!, 
                Localizer.Messages.Get(BotConstants.Messages.Announcement.NickNameIsNotExistMessage)
                , cancellationToken: cancellationToken);
            return Unit.Value;
        }
        
        var announcement = new Announcement
        {
            TelegramUserId = user.Id,
            IsRemoved = false
        };

        await _dbContext.Announcements.AddAsync(announcement, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        await _actionService.SetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{request.ChatId}",
            new CacheModel
            {
                PreviousCommand = BotConstants.Commands.SellCommand,
                Announcement = new AnnouncementCache
                {
                    Id = announcement.Id
                }
            });
        
        var menu = new ReplyKeyboardMarkup(new List<KeyboardButton>
        {
            new(BotConstants.Commands.CancelAnnouncement)
        })
        {
            ResizeKeyboard = true
        };

        await _client.SendTextMessageAsync(request.ChatId!,
            Localizer.Messages.Get(BotConstants.Messages.Announcement.TextMessage),
            replyMarkup: menu, cancellationToken: cancellationToken);
        
        return Unit.Value;
    }
}