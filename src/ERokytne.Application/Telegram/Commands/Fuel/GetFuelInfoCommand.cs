using ERokytne.Application.Ports.Adapters.Fuel;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ERokytne.Application.Telegram.Commands.Fuel;

public class GetFuelInfoCommand : IRequest
{
    public string? ChatId { get; set; }
}

public class GetFuelInfoCommandHandler : IRequestHandler<GetFuelInfoCommand>
{
    private readonly IFuelAdapter _fuelAdapter;
    private readonly ITelegramBotClient _telegramBot;
    private readonly ApplicationDbContext _dbContext;

    public GetFuelInfoCommandHandler(IFuelAdapter fuelAdapter, ITelegramBotClient telegramBot, ApplicationDbContext dbContext)
    {
        _fuelAdapter = fuelAdapter;
        _telegramBot = telegramBot;
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(GetFuelInfoCommand request, CancellationToken cancellationToken)
    {
        _ = await _dbContext.TelegramUsers
                .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, 
                    cancellationToken)
            ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");
        
        var fuelInfo = await _fuelAdapter.GetFuelInfo();

        await _telegramBot.SendTextMessageAsync(request.ChatId!, fuelInfo, ParseMode.Html,
            cancellationToken: cancellationToken);
        return Unit.Value;
    }
}