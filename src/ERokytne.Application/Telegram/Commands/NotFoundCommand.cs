using MediatR;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands;

public class NotFoundCommand : IRequest
{
    public long ChatId { get; set; }
}

public class NotFoundCommandHandler : IRequestHandler<NotFoundCommand>
{
    private readonly ITelegramBotClient _bot;

    public NotFoundCommandHandler(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public async Task<Unit> Handle(NotFoundCommand request, CancellationToken cancellationToken)
    {
        await _bot.SendTextMessageAsync(request.ChatId, "Команда не найдена.", cancellationToken: cancellationToken);
        return Unit.Value;
    }
}