using ERokytne.Domain.Contracts;

namespace ERokytne.Domain.Entities;

public class SupportMessage : IEntity<Guid>
{
    public Guid Id { get; set; }
    
    public string? Text { get; set; }
    
    public Guid? TelegramUserId { get; set; }
    
    public TelegramUser? TelegramUser { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime? UpdatedOn { get; set; }
}