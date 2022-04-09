using System.Text;
using ERokytne.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace ERokytne.Application.Telegram.Commands.Announcements;

public class MyAnnouncementsCommand : IRequest
{
    public string? ChatId { get; set; }
}

public class MyAnnouncementsCommandHandler : IRequestHandler<MyAnnouncementsCommand>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITelegramBotClient _client;

    public MyAnnouncementsCommandHandler(ApplicationDbContext dbContext, ITelegramBotClient client)
    {
        _dbContext = dbContext;
        _client = client;
    }

    public async Task<Unit> Handle(MyAnnouncementsCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.TelegramUsers.AsNoTracking().Include(e => e.Announcements)
                       .FirstOrDefaultAsync(e => e.ChatId == request.ChatId && !e.IsRemoved, cancellationToken)
                   ?? throw new ArgumentNullException($"User with chat id {request.ChatId} is not found or blocked");

        if (user.Announcements.Any())
        {
            var announcementsText = new StringBuilder();
            var counter = 0;
            foreach (var announcement in user.Announcements)
            {
                counter++;
                var text = announcement.Text?.Length > 20 ? $"{announcement.Text[..20]}..." : announcement.Text;
                announcementsText.Append($"№{counter}. {text} Створено: {announcement.CreatedOn.AddHours(3)}\n");
            }
            
            await _client.SendTextMessageAsync(request.ChatId!, announcementsText.ToString(),
                cancellationToken: cancellationToken);
        }
        else
        {
            await _client.SendTextMessageAsync(request.ChatId!, "У вас немає оголошень 😿",
                cancellationToken: cancellationToken);
        }
        
        return Unit.Value;
    }
}