using ERokytne.Telegram.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace ERokytne.Api.Controllers;

[ApiController]
[Route("api/telegram")]
public class TelegramBotController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TelegramBotController> _logger;
    private readonly ITelegramBotCommandHelper _helper;
    
    public TelegramBotController(IMediator mediator, ILogger<TelegramBotController> logger,
        ITelegramBotCommandHelper helper)
    {
        _mediator = mediator;
        _logger = logger;
        _helper = helper;
    }
    
    [HttpPost]
    public async Task<ActionResult> AcceptMessage([FromBody]Update update)
    {
        var command = await _helper.FindCommand(update);

        try
        {
            if (command is not null)
            {
                await _mediator.Send(command);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }

        return Ok();
    }
}