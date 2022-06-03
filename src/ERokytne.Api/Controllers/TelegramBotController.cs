using ERokytne.Application.Telegram.Commands;
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

        if (command is null)
        {
            return Ok();
        }
        
        try
        {
            await _mediator.Send(command);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            
            await _mediator.Send(new SomeErrorCommand
            {
                ChatId = update.CallbackQuery != null
                    ? update.CallbackQuery.Message!.Chat.Id.ToString()
                    : update.Message!.Chat.Id.ToString()
            });
        }

        return Ok();
    }
}