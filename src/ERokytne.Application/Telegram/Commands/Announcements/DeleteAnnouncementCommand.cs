using ERokytne.Domain.Constants;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ERokytne.Application.Telegram.Commands.Announcements;

public class DeleteAnnouncementCommand : IRequest
{
    public string ChatId { get; set; }
    
    public Guid? Id { get; set; }
    
    public int MessageId { get; set; }
}

public class DeleteAnnouncementCommandHandler : IRequestHandler<DeleteAnnouncementCommand>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITelegramBotClient _client;
    private readonly ILogger<DeleteAnnouncementCommandHandler> _logger;

    public DeleteAnnouncementCommandHandler(ApplicationDbContext dbContext, ITelegramBotClient client, 
        ILogger<DeleteAnnouncementCommandHandler> logger)
    {
        _dbContext = dbContext;
        _client = client;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers
                       .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, cancellationToken)
                   ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");

        var announcement =
            await _dbContext.Announcements.Include(e => e.Group
            ).FirstOrDefaultAsync(e => e.Id == request.Id && e.TelegramUserId == user.Id,
                cancellationToken) ?? 
            throw new ArgumentNullException($"Announcement with id {request.Id} is not found");

        announcement.IsRemoved = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var menu = new InlineKeyboardMarkup(new List<IEnumerable<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                new("⬅️ Назад")
                {
                    CallbackData = BotConstants.Commands.CurrentAnnouncementsList
                }
            }
        });

        foreach (var message in announcement.Payload!)
        {
            try
            {
                await _client.DeleteMessageAsync(announcement.Group?.ExternalId!, message, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        await _client.EditMessageTextAsync(request.ChatId, request.MessageId,"Оголошення успішно видалено! ✅", 
            replyMarkup: menu, cancellationToken: cancellationToken);
        
        return Unit.Value;
    }
}