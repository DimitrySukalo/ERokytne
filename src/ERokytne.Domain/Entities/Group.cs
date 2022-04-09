using ERokytne.Domain.Contracts;
using ERokytne.Domain.Enums;

namespace ERokytne.Domain.Entities;

public class Group : IEntity<Guid>
{
    public Guid Id { get; set; }

    public string? ExternalId { get; set; }
    
    public GroupType Type { get; set; }
    
    public bool IsConfirmed { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime? UpdatedOn { get; set; }
}