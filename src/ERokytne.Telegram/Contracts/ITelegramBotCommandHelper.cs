using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ERokytne.Telegram.Contracts;

public interface ITelegramBotCommandHelper
{
    public IBaseRequest FindCommand(Update update, ITelegramBotClient botClient);
}