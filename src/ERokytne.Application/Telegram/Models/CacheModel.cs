namespace ERokytne.Application.Telegram.Models;

public class CacheModel
{
    public string? PreviousCommand { get; set; }
    
    public AnnouncementCache Announcement { get; set; }
    
    public WeatherCache Weather { get; set; }
}