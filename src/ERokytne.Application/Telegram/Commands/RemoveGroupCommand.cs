using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands;

public class RemoveGroupCommand : IRequest
{
    public long GroupId { get; set; }
    
    public long UserId { get; set; }
}

public class RemoveGroupCommandHandler : IRequestHandler<RemoveGroupCommand>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITelegramBotClient _bot;

    public RemoveGroupCommandHandler(ApplicationDbContext dbContext, ITelegramBotClient bot)
    {
        _dbContext = dbContext;
        _bot = bot;
    }

    public async Task<Unit> Handle(RemoveGroupCommand request, CancellationToken cancellationToken)
    {
        if (_bot.BotId != request.UserId)
        {
            return Unit.Value;
        }
        
        var group = await _dbContext.Groups.FirstOrDefaultAsync(e => e.ExternalId == 
                                                                     request.GroupId.ToString(), 
            cancellationToken);

        if (group is null)
        {
            return Unit.Value;
        }
        
        _dbContext.Remove(group);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}