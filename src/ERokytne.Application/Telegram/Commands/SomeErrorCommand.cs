using ERokytne.Application.Localization;
using ERokytne.Domain.Constants;
using MediatR;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands;

public class SomeErrorCommand : IRequest
{
    public string ChatId { get; set; }
}

public class SomeErrorCommandHandler : IRequestHandler<SomeErrorCommand>
{
    private readonly ITelegramBotClient _botClient;

    public SomeErrorCommandHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task<Unit> Handle(SomeErrorCommand request, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessageAsync(request.ChatId, 
            Localizer.Messages.Get(BotConstants.Messages.SomeError.Message), cancellationToken: cancellationToken);
        
        return Unit.Value;
    }
}