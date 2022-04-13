using ERokytne.Application.Helpers;
using ERokytne.Application.Localization;
using ERokytne.Domain.Constants;
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
                KeyboardButton.WithRequestContact(Localizer.Messages.Get(
                    BotConstants.Messages.StartCommand.SharePhoneNumber)))
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };

            await _bot.SendTextMessageAsync(request.ChatId, Localizer.Messages.Get(
                BotConstants.Messages.StartCommand.ConfirmPhoneByButton),
                replyMarkup: phoneKeyboard, cancellationToken: cancellationToken);
        }
        else
        {
            await _bot.SendTextMessageAsync(request.ChatId, Localizer.Messages.Get(
                    BotConstants.Messages.StartCommand.MainMenu)
                ,replyMarkup: UserCommandHelper.GetStartMenu(), parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
        }
        
        
        return Unit.Value;
    }
}