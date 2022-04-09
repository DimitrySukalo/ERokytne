using ERokytne.Domain.Entities;
using ERokytne.Domain.Enums;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands.Groups;

public class AddGroupCommand : IRequest
{
    public long GroupId { get; set; }
    
    public long UserId { get; set; }
}

public class AddGroupCommandHandler : IRequestHandler<AddGroupCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _dbContext;

    public AddGroupCommandHandler(ITelegramBotClient bot, ApplicationDbContext dbContext)
    {
        _bot = bot;
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(AddGroupCommand request, CancellationToken cancellationToken)
    {
        if (_bot.BotId != request.UserId)
        {
            return Unit.Value;
        }
        
        var group = await _dbContext.Groups.FirstOrDefaultAsync(e =>
            e.ExternalId == request.GroupId.ToString(), cancellationToken);

        if (group is not null)
        {
            return Unit.Value;
        }
            
        group = new Group
        {
            ExternalId = request.GroupId.ToString(),
            IsConfirmed = false,
            Type = GroupType.Announcement
        };

        await _dbContext.AddAsync(group, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}