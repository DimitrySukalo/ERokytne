using ERokytne.Application.Cache;
using ERokytne.Application.Telegram.Models;
using ERokytne.Domain.Constants;
using ERokytne.Domain.Entities;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ERokytne.Application.Telegram.Commands.Announcements;

public class SellCommand : IRequest
{
    public string? ChatId { get; set; }
    
    public string? NickName { get; set; }
}

public class SellCommandHandler : IRequestHandler<SellCommand>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserActionService _actionService;
    private readonly ITelegramBotClient _client;

    public SellCommandHandler(ApplicationDbContext dbContext, UserActionService actionService, ITelegramBotClient client)
    {
        _dbContext = dbContext;
        _actionService = actionService;
        _client = client;
    }

    public async Task<Unit> Handle(SellCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers.Include(e => e.Announcements)
            .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, cancellationToken)
                   ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");

        if (user.Announcements.Count(e => e.CreatedOn.Date == DateTime.UtcNow.Date) >= 3)
        {
            await _client.SendTextMessageAsync(request.ChatId!,
                "Ви витратили ліміт на сьогодні 😿. Якщо хочете опублікувати нове оголошення, видаліть одне з своїх попередніх 😺",
                cancellationToken: cancellationToken);
            
            return Unit.Value;
        }
        
        user.NickName = $"@{request.NickName}";
        var announcement = new Announcement
        {
            TelegramUserId = user.Id
        };

        await _dbContext.Announcements.AddAsync(announcement, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        await _actionService.SetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{request.ChatId}",
            new AnnouncementCacheModel
            {
                PreviousCommand = BotConstants.Commands.SellCommand,
                Id = announcement.Id
            });
        
        var menu = new ReplyKeyboardMarkup(new List<KeyboardButton>
        {
            new(BotConstants.Commands.CancelAnnouncement)
        })
        {
            ResizeKeyboard = true
        };

        await _client.SendTextMessageAsync(request.ChatId!,"☝️ Зауважте, якщо у вас немає тегу, то для ідентифікації оголошення буде використаний ваш номер телефону. " +
                                                           "Також вам доступно опублікувати максимум 3 оголошення на день 👌\n️" 
                                                           + "Введіть текст, який буде відображений у вашому оголошенні 💬 ",
            replyMarkup: menu, cancellationToken: cancellationToken);
        
        return Unit.Value;
    }
}