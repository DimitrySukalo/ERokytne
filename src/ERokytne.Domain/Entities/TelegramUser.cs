using ERokytne.Domain.Contracts;

namespace ERokytne.Domain.Entities;

public class TelegramUser : IEntity<Guid>
{
    public Guid Id { get; set; }
    
    public string? FullName { get; set; }
    
    public string? NickName { get; set; }
    
    public string? ChatId { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public bool IsRemoved { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime? UpdatedOn { get; set; }
}