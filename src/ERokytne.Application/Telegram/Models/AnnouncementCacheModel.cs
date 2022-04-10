namespace ERokytne.Application.Telegram.Models;

public class AnnouncementCacheModel
{
    public string? PreviousCommand { get; set; }

    public Guid? Id { get; set; }
    
    public int? PageIndex { get; set; }
    
    public int? MessageId { get; set; }
}