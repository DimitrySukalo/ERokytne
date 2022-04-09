using ERokytne.Domain.Entities;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands.Announcements;

public class AddAnnouncementPhotosCommand : IRequest
{
    public string? ChatId { get; set; }
    
    public Guid? AnnouncementId { get; set; }
    
    public string? FileId { get; set; }
}

public class AddAnnouncementPhotosCommandHandler : IRequestHandler<AddAnnouncementPhotosCommand>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITelegramBotClient _client;

    public AddAnnouncementPhotosCommandHandler(ApplicationDbContext dbContext, ITelegramBotClient client)
    {
        _dbContext = dbContext;
        _client = client;
    }

    public async Task<Unit> Handle(AddAnnouncementPhotosCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers
                       .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, cancellationToken)
                   ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");

        var announcement =
            await _dbContext.Announcements.FirstOrDefaultAsync(e => 
                    e.Id == request.AnnouncementId && e.TelegramUserId == user.Id,
                cancellationToken) ?? 
            throw new ArgumentNullException($"Announcement with id {request.AnnouncementId} is not found");
        
        
        announcement.Photos.Add(new Photo
        {
            Path = request.FileId
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}