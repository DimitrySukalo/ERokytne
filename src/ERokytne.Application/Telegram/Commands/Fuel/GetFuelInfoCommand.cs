using ERokytne.Application.Ports.Adapters.Fuel;
using MediatR;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands.Fuel;

public class GetFuelInfoCommand : IRequest
{
    public string? ChatId { get; set; }
}

public class GetFuelInfoCommandHandler : IRequestHandler<GetFuelInfoCommand>
{
    private readonly IFuelAdapter _fuelAdapter;
    private readonly ITelegramBotClient _telegramBot;

    public GetFuelInfoCommandHandler(IFuelAdapter fuelAdapter, ITelegramBotClient telegramBot)
    {
        _fuelAdapter = fuelAdapter;
        _telegramBot = telegramBot;
    }

    public async Task<Unit> Handle(GetFuelInfoCommand request, CancellationToken cancellationToken)
    {
        var fuelInfo = await _fuelAdapter.GetFuelInfo();
        return Unit.Value;
    }
}