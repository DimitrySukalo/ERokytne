using MediatR;
using Telegram.Bot.Types;

namespace ERokytne.Telegram.Contracts;

public interface ITelegramBotCommandHelper
{
    public Task<IBaseRequest?> FindCommand(Update update);
}