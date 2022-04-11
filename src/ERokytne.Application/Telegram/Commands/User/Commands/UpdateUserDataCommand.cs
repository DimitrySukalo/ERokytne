using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands.User.Commands;

public class UpdateUserDataCommand : IRequest
{
    public string? FullName { get; set; }
    
    public string? NickName { get; set; }
    
    public string ChatId { get; set; }
}

public class UpdateUserDataCommandHandler : IRequestHandler<UpdateUserDataCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _dbContext;

    public UpdateUserDataCommandHandler(ITelegramBotClient bot, ApplicationDbContext dbContext)
    {
        _bot = bot;
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(UpdateUserDataCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers
                .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, 
                    cancellationToken)
            ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");

        user.FullName = request.FullName;
        user.NickName = request.NickName;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}