using MediatR;

namespace ERokytne.Application.Telegram.Commands.Announcements;

public class PostAnnouncementCommand : IRequest
{
    public string? ChatId { get; set; }
    
    public Guid? AnnouncementId { get; set; }
}