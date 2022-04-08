namespace ERokytne.Application.Telegram.Models;

public class TelegramUserDto
{
    public long UserId { get; set; }
    
    public string? FirstName { get; set; }
        
    public string? LastName { get; set; }
}