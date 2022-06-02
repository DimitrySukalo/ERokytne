using ERokytne.Application.Telegram.Commands.Weather;
using MassTransit;
using MediatR;

namespace ERokytne.Api.Consumers;

public class SendWeatherCommandConsumer : IConsumer<SendWeatherCommand>
{
    private readonly IMediator _mediator;
    private readonly ILogger<SendWeatherCommandConsumer> _logger;

    public SendWeatherCommandConsumer(IMediator mediator, ILogger<SendWeatherCommandConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
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
}