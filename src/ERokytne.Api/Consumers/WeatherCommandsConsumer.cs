using ERokytne.Application.Telegram.Commands.Weather;
using ERokytne.Persistence;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERokytne.Api.Consumers;

public class WeatherCommandsConsumer : IConsumer<SendWeatherCommand>, IConsumer<UpdateWeatherCommand>
{
    private readonly IMediator _mediator;
    private readonly ILogger<WeatherCommandsConsumer> _logger;
    private readonly ApplicationDbContext _dbContext;

    public WeatherCommandsConsumer(IMediator mediator, ILogger<WeatherCommandsConsumer> logger, ApplicationDbContext dbContext)
    {
        _mediator = mediator;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<SendWeatherCommand> context)
    {
        var totalCount = context.Message.Chats.Count;
        var messages = context.Message.Chats.Take(10).ToList();
        
        _logger.LogInformation("Sending weather notifications: {SendingCount} of {TotalCount}",
            messages.Count, totalCount);
        
        await _mediator.Send(new SendWeatherCommand { Chats = messages });
        
        if (totalCount > 10)
        {
            await context.Send(new SendWeatherCommand
                { Chats = context.Message.Chats.Skip(10).ToList() });
        }

        _logger.LogInformation(
            "Sending push notifications: {SendingCount} of {TotalCount} completed",
            messages.Count, context.Message.Chats.Count);
    }

    public async Task Consume(ConsumeContext<UpdateWeatherCommand> context)
    {
        var chats = await _dbContext.TelegramUsers.AsNoTracking()
            .Where(e => !e.IsRemoved).Select(e => e.ChatId).ToListAsync();

        if (chats.Any())
        {
            await context.Send(new SendWeatherCommand {Chats = chats});
        }
    }
}