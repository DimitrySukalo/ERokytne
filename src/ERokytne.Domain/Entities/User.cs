using ERokytne.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ERokytne.Domain.Entities;

public class User : IdentityUser
{
    public string? TelegramChatId { get; set; }
    
    public UserType UserType { get; set; }
}