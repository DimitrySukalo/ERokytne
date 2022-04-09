using ERokytne.Domain.Contracts;

namespace ERokytne.Domain.Entities;

public class Announcement : IEntity<Guid>
{
    public Guid Id { get; set; }
    
    public string? Text { get; set; }
    
    public List<Photo> Photos { get; set; }
    
    public Guid TelegramUserId { get; set; }
    
    public TelegramUser TelegramUser { get; set; }

    public DateTime CreatedOn { get; set; }
    
    public DateTime? UpdatedOn { get; set; }
}