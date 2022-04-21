using ERokytne.Application.Cache;
using ERokytne.Application.Localization;
using ERokytne.Application.Paging;
using ERokytne.Application.Telegram.Models;
using ERokytne.Domain.Constants;
using ERokytne.Domain.Entities;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ERokytne.Application.Telegram.Commands.Announcements;

public class MyAnnouncementsCommand : IRequest
{
    public string? ChatId { get; set; }
    
    public int PageIndex { get; set; }
    
    public int? MessageId { get; set; }
}

public class MyAnnouncementsCommandHandler : IRequestHandler<MyAnnouncementsCommand>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITelegramBotClient _client;
    private readonly UserActionService _actionService;

    public MyAnnouncementsCommandHandler(ApplicationDbContext dbContext, ITelegramBotClient client, 
        UserActionService actionService)
    {
        _dbContext = dbContext;
        _client = client;
        _actionService = actionService;
    }

    public async Task<Unit> Handle(MyAnnouncementsCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers.AsNoTracking()
                       .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, cancellationToken)
                   ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");

        var announcements = _dbContext.Announcements.Where(e => e.TelegramUserId == user.Id
         && !e.IsRemoved);
        var paginatedAnnouncements = await PaginatedList<Announcement>.CreateAsync(announcements,
            request.PageIndex, 5);

        if (paginatedAnnouncements.Count == 0 && paginatedAnnouncements.HasPreviousPage)
        {
            request.PageIndex -= 1;
            paginatedAnnouncements = await PaginatedList<Announcement>.CreateAsync(announcements,
                request.PageIndex, 5);
        }
        
        if (paginatedAnnouncements.Any())
        {
            var counter = request.PageIndex == 1 ? 0 : 5 * request.PageIndex - 5;
            var keys = new List<List<InlineKeyboardButton>>();
            foreach (var announcement in paginatedAnnouncements)
            {
                counter++;
                var text = announcement.Text?.Length > 15 ? $"{announcement.Text[..15]}..." : announcement.Text;

                keys.Add(new List<InlineKeyboardButton>
                {
                    new($"№{counter}. {text}\n")
                    {
                        CallbackData = announcement.Id.ToString()
                    }
                });
            }

            var pagingKeys = new List<InlineKeyboardButton>();

            if (paginatedAnnouncements.HasPreviousPage)
            {
                pagingKeys.Add(new InlineKeyboardButton(
                    Localizer.Messages.Get(BotConstants.Messages.Announcement.PreviousPageMessage))
                {
                    CallbackData = BotConstants.Commands.PreviousAnnouncementsList
                });
            }
            
            if (paginatedAnnouncements.HasNextPage)
            {
                pagingKeys.Add(new InlineKeyboardButton(
                    Localizer.Messages.Get(BotConstants.Messages.Announcement.NextPageMessage))
                {
                    CallbackData = BotConstants.Commands.NextAnnouncementsList
                });
            }

            keys.Add(pagingKeys);
            int? messageId;
            
            if (request.PageIndex > 1 || request.MessageId.HasValue)
            {
                var parsedMessageId = int.Parse(request.MessageId.ToString()!);
                await RewriteMessage(request.ChatId, keys, parsedMessageId, cancellationToken);

                messageId = parsedMessageId;
            }
            else
            {
                messageId = await SendMessage(request.ChatId, keys, cancellationToken);
            }
            
            await _actionService.SetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{request.ChatId}",
                new CacheModel
                {
                    Announcement = new AnnouncementCache
                    {
                        PageIndex = request.PageIndex,
                        MessageId = messageId,
                    },
                    PreviousCommand = BotConstants.Commands.MyAnnouncementsCommand
                });
        }
        else
        {
            await _actionService.DeleteUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{request.ChatId}");
            await _client.SendTextMessageAsync(request.ChatId!,
                Localizer.Messages.Get(BotConstants.Messages.Announcement.IsNotExistMessage),
                cancellationToken: cancellationToken);
        }
        
        return Unit.Value;
    }

    private async Task<int> SendMessage(string? chatId, IEnumerable<List<InlineKeyboardButton>> keys, 
        CancellationToken cancellationToken)
    {
        var message = await _client.SendTextMessageAsync(chatId!, 
            Localizer.Messages.Get(BotConstants.Messages.Announcement.SelectMessage), 
            replyMarkup: new InlineKeyboardMarkup(keys),
            cancellationToken: cancellationToken);

        return message.MessageId;
    }
    
    private async Task RewriteMessage(string? chatId, IEnumerable<List<InlineKeyboardButton>> keys, int messageId,
        CancellationToken cancellationToken)
    {
        await _client.EditMessageTextAsync(chatId!, messageId, 
            Localizer.Messages.Get(BotConstants.Messages.Announcement.SelectMessage)
            ,replyMarkup: new InlineKeyboardMarkup(keys), cancellationToken: cancellationToken);
    }
}