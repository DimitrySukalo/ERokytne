using ERokytne.Domain.Contracts;
using ERokytne.Domain.Enums;

namespace ERokytne.Domain.Entities;

public class Photo : IEntity<Guid>
{
    public Guid Id { get; set; }
    
    public string? Path { get; set; }
    
    public GroupType Type { get; set; }
    
    public Guid? AnnouncementId { get; set; }
    
    public Announcement? Announcement { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime? UpdatedOn { get; set; }
}