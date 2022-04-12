using ERokytne.Application.Telegram.Commands.Notifications;
using MassTransit;
using MediatR;

namespace ERokytne.Api.Consumers;

public class SendNotificationsCommandConsumer : IConsumer<SendNotificationsCommand>
{
    private readonly IMediator _mediator;

    public SendNotificationsCommandConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<SendNotificationsCommand> context) =>
        await _mediator.Send(context.Message);
}