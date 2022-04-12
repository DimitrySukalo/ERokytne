using ERokytne.Application.Telegram.Commands.Notifications;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERokytne.Api.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly ISendEndpointProvider _sendEndpoint;
    private readonly IMediator _mediator;

    public AdminController(ISendEndpointProvider sendEndpoint, IMediator mediator)
    {
        _sendEndpoint = sendEndpoint;
        _mediator = mediator;
    }

    [HttpGet]
    public Task<IActionResult> Index() =>
        Task.FromResult<IActionResult>(View("Index"));
    
    [HttpGet]
    public Task<IActionResult> NotificationIndex() =>
        Task.FromResult<IActionResult>(View("Notifications/Index"));
    
    [HttpGet]
    public Task<IActionResult> NotificationToAllIndex() =>
        Task.FromResult<IActionResult>(View("Notifications/SendNotificationToAll"));
    
    [HttpGet]
    public Task<IActionResult> NotificationToOneIndex() =>
        Task.FromResult<IActionResult>(View("Notifications/SendOneNotification"));

    [HttpPost]
    public async Task<string> SendNotifications(string text)
    {
        await _sendEndpoint.Send(new SendNotificationsCommand
        {
            Text = text
        });

        return "Повідомлення успішно надіслані";
    }
    
    [HttpPost]
    public async Task<string> SendNotification(string text, long chatId)
    {
        await _mediator.Send(new SendNotificationCommand
        {
            Text = text,
            ChatId = chatId
        });

        return "Повідомлення успішно надіслано";
    }
}