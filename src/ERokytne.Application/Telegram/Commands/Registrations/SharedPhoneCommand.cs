using System.Text;
using ERokytne.Application.Helpers;
using ERokytne.Domain.Entities;
using ERokytne.Domain.Enums;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

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
            
            await _bot.SendTextMessageAsync(request.ChatId, GetMessage(),
                replyMarkup: UserCommandHelper.GetStartMenu(), parseMode: ParseMode.Html, 
                cancellationToken: cancellationToken);
            
            return Unit.Value;
        }
        
        await _bot.SendTextMessageAsync(request.ChatId, "Ви уже зареєстровані"
            ,replyMarkup: UserCommandHelper.GetStartMenu(), cancellationToken: cancellationToken);

        return Unit.Value;
    }
    
    private static string GetMessage()
    {
        var message = new StringBuilder();

        message.Append("⚡️Цей бот створений для того, щоб люди могли швидко отримувати потрібну їм інформацію.\n" +
                       "Він має 3 розділи:\n");
        message.Append(" \n");
        message.Append(
            "📑 Оголошення:\n- Тут ви можете подати своє оголошення на вашу тему.\n" +
            "Його будуть бачити всі учасники каналу.\nЯк створити оголошення? Дуже просто!\n");
        message.Append(" \n");
        message.Append("<b>Створити оголошення -> Вводите текст і відправляєте його -> " +
                       "По бажанню завантажуєте фото ( будь-яка кількість) -> Опублікувати</b> ✅\n");
        message.Append(" \n");
        message.Append("💻 Мої оголошення:\n");
        message.Append("- Тут ви можете переглядати та видаляти свої оголошення.\nЯк видалити оголошення?\n");
        message.Append(" \n");
        message.Append("<b>Мої оголошення -> Обираєте ваше оголошення -> Видалити оголошення</b> ✅\n");
        message.Append(" \n");
        message.Append("🙌 Підтримка\n");
        message.Append("- Тут ви можете написати своє запитання або запропонувати ідею, як покращити бота. " +
                       "Якщо ідея буде дійсно крута, то отримаєте від нас бонус!\nЯк відправити повідомлення в підтримку?");
        message.Append(" \n");
        message.Append("<b>Підтримка -> Вводите текст та відправляєте -> Готово</b> ✅\n");
        message.Append(" \n");
        message.Append("Бажаємо вам приємного користування! Ваш функціонал нижче 👇");

        return message.ToString();
    }
}