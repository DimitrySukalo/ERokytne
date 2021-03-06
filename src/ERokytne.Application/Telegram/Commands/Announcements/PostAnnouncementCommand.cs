using System.Text;
using ERokytne.Application.Cache;
using ERokytne.Application.Helpers;
using ERokytne.Application.Localization;
using ERokytne.Application.Telegram.Models;
using ERokytne.Domain.Constants;
using ERokytne.Domain.Enums;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

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

    public PostAnnouncementCommandHandler(ApplicationDbContext dbContext, UserActionService actionService, 
        ITelegramBotClient client)
    {
        _dbContext = dbContext;
        _actionService = actionService;
        _client = client;
    }

    public async Task<Unit> Handle(PostAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers.AsNoTracking()
                       .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, cancellationToken)
                   ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");

        var announcement =
            await _dbContext.Announcements.Include(e => e.Photos
            ).FirstOrDefaultAsync(e => e.Id == request.AnnouncementId && e.TelegramUserId == user.Id,
                cancellationToken) ?? 
            throw new ArgumentNullException($"Announcement with id {request.AnnouncementId} is not found");
        
        var announcementGroup = await _dbContext.Groups
                                    .FirstOrDefaultAsync(e => e.IsConfirmed && e.Type == GroupType.Announcement, cancellationToken)
                                ?? throw new ArgumentNullException("Announcement confirmed group is not found");

        var text = new StringBuilder();
        text.Append($"{announcement.Text}\n");
        text.Append($"????????????????????: {user.NickName}");

        var externalIds = new List<int>();
        if (announcement.Photos.Count > 0)
        {
            using var photos = new StreamCollection();
            var media = new List<IAlbumInputMedia>();
            for (var i = 0; i < announcement.Photos.Count; i++)
            {
                photos.Add(new MemoryStream());
                await _client.GetInfoAndDownloadFileAsync(announcement.Photos[i].Path!, photos[i],
                    cancellationToken);
                photos[i].Position = 0;
            }

            for (var i = 0; i < photos.Count; i++)
            {
                InputMediaPhoto photo;
                if (i == 0)
                {
                    photo = new InputMediaPhoto(new InputMedia(photos[i], Guid.NewGuid().ToString()))
                    {
                        Caption = text.ToString()
                    };
                }
                else
                {
                    photo = new InputMediaPhoto(new InputMedia(photos[i], Guid.NewGuid().ToString()));
                }

                media.Add(photo);
            }
                
            var messages = await _client.SendMediaGroupAsync(announcementGroup.ExternalId!, media,
                cancellationToken: cancellationToken);

            externalIds.AddRange(messages.Select(e => e.MessageId));
        }
        else
        {
            var message = await _client.SendTextMessageAsync(announcementGroup.ExternalId!, text.ToString(), 
                cancellationToken: cancellationToken);

            externalIds.Add(message.MessageId);
        }

        announcement.Payload?.AddRange(externalIds);
        announcement.GroupId = announcementGroup.Id;
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        await _actionService.DeleteUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{request.ChatId}");
        await _client.SendTextMessageAsync(request.ChatId!, 
            Localizer.Messages.Get(BotConstants.Messages.Announcement.IsCreatedMessage)
            ,replyMarkup: UserCommandHelper.GetStartMenu(), cancellationToken: cancellationToken);
        
        return Unit.Value;
    }
}