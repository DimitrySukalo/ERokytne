using ERokytne.Application.Helpers;
using ERokytne.Domain.Entities;
using ERokytne.Domain.Enums;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands.Registrations;

public class SharedPhoneCommand : IRequest
{
    public long ChatId { get; set; }
    
    public string? Phone { get; set; }
    
    public string? NickName { get; set; }
    
    public string? FullName { get; set; }
}

public class SharedPhoneCommandHandler : IRequestHandler<SharedPhoneCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _dbContext;

    public SharedPhoneCommandHandler(ITelegramBotClient bot, ApplicationDbContext dbContext)
    {
        _bot = bot;
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(SharedPhoneCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers
            .FirstOrDefaultAsync(e => e.PhoneNumber == request.Phone && !e.IsRemoved,
            cancellationToken);

        if (user is null)
        {
            user = new TelegramUser
            {
                PhoneNumber = request.Phone,
                ChatId = request.ChatId.ToString(),
                IsRemoved = false,
                NickName = request.NickName,
                FullName = request.FullName,
                Type = TelegramUserType.User
            };

            await _dbContext.TelegramUsers.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            await _bot.SendTextMessageAsync(request.ChatId, "Ви успішно зареєструвалися. Ваші можливості нижче ⬇️",
                replyMarkup: UserCommandHelper.GetStartMenu(), cancellationToken: cancellationToken);
            
            return Unit.Value;
        }
        
        await _bot.SendTextMessageAsync(request.ChatId, "Ви уже зареєстровані"
            ,replyMarkup: UserCommandHelper.GetStartMenu(), cancellationToken: cancellationToken);

        return Unit.Value;
    }
}