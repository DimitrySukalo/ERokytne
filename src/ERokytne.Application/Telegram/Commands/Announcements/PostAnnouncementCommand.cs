using ERokytne.Application.Cache;
using ERokytne.Application.Telegram.Models;
using ERokytne.Domain.Constants;
using ERokytne.Domain.Enums;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ERokytne.Application.Telegram.Commands.Announcements;

public class PostAnnouncementCommand : IRequest
{
    public string? ChatId { get; set; }
    
    public Guid? AnnouncementId { get; set; }
}

public class PostAnnouncementCommandHandler : IRequestHandler<PostAnnouncementCommand>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserActionService _actionService;
    private readonly ITelegramBotClient _client;
    private readonly ILogger<PostAnnouncementCommandHandler> _logger;

    public PostAnnouncementCommandHandler(ApplicationDbContext dbContext, UserActionService actionService, 
        ITelegramBotClient client, ILogger<PostAnnouncementCommandHandler> logger)
    {
        _dbContext = dbContext;
        _actionService = actionService;
        _client = client;
        _logger = logger;
    }

    public async Task<Unit> Handle(PostAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers.AsNoTracking()
                       .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, cancellationToken)
                   ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");

        var announcement =
            await _dbContext.Announcements.AsNoTracking().Include(e => e.Photos
            ).FirstOrDefaultAsync(e => e.Id == request.AnnouncementId && e.TelegramUserId == user.Id,
                cancellationToken) ?? 
            throw new ArgumentNullException($"Announcement with id {request.AnnouncementId} is not found");
        
        var announcementGroup = await _dbContext.Groups.AsNoTracking()
                                    .FirstOrDefaultAsync(e => e.IsConfirmed && e.Type == GroupType.Announcement, cancellationToken)
                                ?? throw new ArgumentNullException("Announcement confirmed group is not found");

        using (var photos = new StreamCollection())
        {
            var media = new List<IAlbumInputMedia>();
            for (var i = 0; i < announcement.Photos.Count; i++)
            {
                photos.Add(new MemoryStream());
                await _client.GetInfoAndDownloadFileAsync(announcement.Photos[i].Path!, photos[i] , cancellationToken);
                photos[i].Position = 0;
            }

            media.AddRange(photos
                .Select(photo => new InputMediaPhoto(new InputMedia(photo, Guid.NewGuid().ToString()))));
            
            if (media.Count > 0)
            {
                await _client.SendMediaGroupAsync(announcementGroup.ExternalId!, media,
                    cancellationToken: cancellationToken);
            }
        }
        
        await _client.SendTextMessageAsync(announcementGroup.ExternalId!, announcement.Text!, 
            cancellationToken: cancellationToken);

        await _actionService.DeleteUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{request.ChatId}");
        
        var menu = new ReplyKeyboardMarkup(new List<KeyboardButton>
        {
            new(BotConstants.Commands.SellCommand)
        })
        {
            ResizeKeyboard = true
        };
            
        await _client.SendTextMessageAsync(request.ChatId!, "Оголошення успішно створено!"
            ,replyMarkup: menu, cancellationToken: cancellationToken);
        
        return Unit.Value;
    }
}