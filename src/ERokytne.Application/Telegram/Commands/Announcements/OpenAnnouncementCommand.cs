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

public class OpenAnnouncementCommand : IRequest
{
    public Guid Id { get; set; }
    
    public string ChatId { get; set; }
    
    public int MessageId { get; set; }
}

public class OpenAnnouncementCommandHandler : IRequestHandler<OpenAnnouncementCommand>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITelegramBotClient _client;
    private readonly UserActionService _actionService;

    public OpenAnnouncementCommandHandler(ApplicationDbContext dbContext, ITelegramBotClient client, 
        UserActionService actionService)
    {
        _dbContext = dbContext;
        _client = client;
        _actionService = actionService;
    }

    public async Task<Unit> Handle(OpenAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers.AsNoTracking()
                               .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, cancellationToken)
                           ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");
        
        var announcement = await _dbContext.Announcements.AsNoTracking().Include(e => e.Photos)
                              .FirstOrDefaultAsync(e => e.Id == request.Id && e.TelegramUserId == user.Id,
                cancellationToken) ?? 
            throw new ArgumentNullException($"Announcement with id {request.Id} is not found");

        var menu = new InlineKeyboardMarkup(new List<IEnumerable<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                new(BotConstants.Commands.DeleteAnnouncement)
                {
                    CallbackData = announcement.Id.ToString()
                }
            },
            new List<InlineKeyboardButton>
            {
                new(Localizer.Messages.Get(BotConstants.Messages.Announcement.BackToListMessage))
                {
                    CallbackData = BotConstants.Commands.CurrentAnnouncementsList
                }
            }
        });

        var lastCommand = await _actionService
            .GetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{request.ChatId}",
                () => Task.FromResult(new CacheModel()));

        lastCommand.PreviousCommand = BotConstants.Commands.OpenAnnouncementCommand;
        await _actionService.SetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{request.ChatId}", lastCommand);
        
        await _client.EditMessageTextAsync(request.ChatId, request.MessageId,
            Localizer.Messages.Get(BotConstants.Messages.Announcement.SelectActionMessage), replyMarkup: menu,
            cancellationToken: cancellationToken);
        
        return Unit.Value;
    }
}
