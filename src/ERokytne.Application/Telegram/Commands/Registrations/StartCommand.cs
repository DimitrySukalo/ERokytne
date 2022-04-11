using ERokytne.Application.Helpers;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ERokytne.Application.Telegram.Commands.Registrations;

public class StartCommand : IRequest
{
    public long ChatId { get; set; }
}

public class StartCommandHandler : IRequestHandler<StartCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _dbContext;

    public StartCommandHandler(ITelegramBotClient bot, ApplicationDbContext dbContext)
    {
        _bot = bot;
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(StartCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers.AsNoTracking()
            .FirstOrDefaultAsync(e => e.ChatId == request.ChatId.ToString(), cancellationToken);

        if (user is null)
        {
            var phoneKeyboard = new ReplyKeyboardMarkup(
                KeyboardButton.WithRequestContact("Поділитися номером телефону ☎️"))
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };

            await _bot.SendTextMessageAsync(request.ChatId, "Підтвердіть номер за допомогою кнопки нижче ⬇️",
                replyMarkup: phoneKeyboard, cancellationToken: cancellationToken);
        }
        else
        {
            await _bot.SendTextMessageAsync(request.ChatId, "👨‍🚀 Ви у головному меню."
                ,replyMarkup: UserCommandHelper.GetStartMenu(), parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
        }
        
        
        return Unit.Value;
    }
}