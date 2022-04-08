using Telegram.Bot.Types.Enums;

namespace ERokytne.Application.Telegram.Models;

public class TelegramMessageDto
{
    public long ChatId { get; set; }
        
    public string? Text { get; set; }
        
    public TelegramUserDto UserDto { get; set; }
    
    public MessageType Type { get; set; }
}