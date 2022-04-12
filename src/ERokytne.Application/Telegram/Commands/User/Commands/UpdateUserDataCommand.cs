using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERokytne.Application.Telegram.Commands.User.Commands;

public class UpdateUserDataCommand : IRequest
{
    public string? FullName { get; set; }
    
    public string? NickName { get; set; }
    
    public string ChatId { get; set; }
}

public class UpdateUserDataCommandHandler : IRequestHandler<UpdateUserDataCommand>
{
    private readonly ApplicationDbContext _dbContext;

    public UpdateUserDataCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(UpdateUserDataCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers
            .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved,
                cancellationToken);

        if (user is null)
        {
            return Unit.Value;
        }
        
        user.FullName = request.FullName;
        user.NickName = request.NickName;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}