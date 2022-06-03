using ERokytne.Domain.Contracts;
using ERokytne.Domain.Enums;

namespace ERokytne.Domain.Entities;

public class TelegramUser : IEntity<Guid>
{
    public Guid Id { get; set; }
    
    public string? FullName { get; set; }
    
    public string? NickName { get; set; }
    
    public string? ChatId { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public TelegramUserType Type { get; set; }

    public List<Announcement> Announcements { get; set; } = new();

    public List<SupportMessage> SupportMessages { get; set; } = new();

    public List<Job> Jobs { get; set; } = new();
        
    public bool IsRemoved { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime? UpdatedOn { get; set; }
}