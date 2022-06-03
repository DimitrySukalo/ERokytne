using ERokytne.Domain.Contracts;
using ERokytne.Domain.Enums;

namespace ERokytne.Domain.Entities;

public class Job : IEntity<Guid>
{
    public Guid Id { get; set; }
    
    public JobType Type { get; set; }
    
    public bool IsActivated { get; set; }
    
    public Guid TelegramUserId { get; set; }
    
    public TelegramUser TelegramUser { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime? UpdatedOn { get; set; }
}