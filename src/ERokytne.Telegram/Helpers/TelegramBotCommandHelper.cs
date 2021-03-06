using ERokytne.Application.Cache;
using ERokytne.Application.Helpers;
using ERokytne.Application.Telegram.Commands.Announcements;
using ERokytne.Application.Telegram.Commands.Fuel;
using ERokytne.Application.Telegram.Commands.Groups;
using ERokytne.Application.Telegram.Commands.Registrations;
using ERokytne.Application.Telegram.Commands.Subscriptions;
using ERokytne.Application.Telegram.Commands.Support.Commands;
using ERokytne.Application.Telegram.Commands.User.Commands;
using ERokytne.Application.Telegram.Commands.Weather;
using ERokytne.Application.Telegram.Models;
using ERokytne.Domain.Constants;
using ERokytne.Telegram.Contracts;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ERokytne.Telegram.Helpers;

public class TelegramBotCommandHelper : ITelegramBotCommandHelper
{
    private readonly ITelegramBotClient _bot;
    private readonly UserActionService _service;
    private readonly IMediator _mediator;

    public TelegramBotCommandHelper(ITelegramBotClient bot, UserActionService service, IMediator mediator)
    {
        _bot = bot;
        _service = service;
        _mediator = mediator;
    }

    public async Task<IBaseRequest?> FindCommand(Update update)
    {
        var message = GetMessageDto(update, _bot);
        if (string.IsNullOrWhiteSpace(message.Text))
        {
            return null!;
        }
        
        await _mediator.Send(new UpdateUserDataCommand
        {
            ChatId = message.ChatId.ToString(),
            FullName = $"{message.UserDto.FirstName} {message.UserDto.LastName}",
            NickName = $"@{message.UserDto.NickName}"
        });

        return message.Type switch
        {
            MessageType.Text when message.Text.Equals(BotConstants.Commands.StartCommand) =>
                await GetRegisterUserCommand(message),
            MessageType.Text when message.Text.Equals(BotConstants.Commands.CancelAnnouncement) =>
                await GetCancelAnnouncementCommand(message),
            MessageType.Text when message.Text.Equals(BotConstants.Commands.MyAnnouncementsCommand) =>
                await GetMyAnnouncementsCommand(message),
            MessageType.Text when message.Text.Equals(BotConstants.Commands.SupportCommand) =>
                await GetSupportCommand(message),
            MessageType.Contact => await GetContactConfirmedCommand(message),
            MessageType.ChatMembersAdded => await GetAddGroupCommand(message),
            MessageType.ChatMemberLeft => await GetRemoveGroupCommand(message),
            MessageType.Text when message.Text.Equals(BotConstants.Commands.SellCommand) =>
                await GetSellCarCommand(message),
            MessageType.Text when message.Text.Equals(BotConstants.Commands.PostAnnouncement) =>
                await GetPostAnnouncementCommand(message),
            MessageType.Text when message.Text.Equals(BotConstants.Commands.NextAnnouncementsList) => 
                await GetNextAnnouncementListCommand(message),
            MessageType.Text when message.Text.Equals(BotConstants.Commands.PreviousAnnouncementsList) => 
                await GetPreviousAnnouncementListCommand(message),
            MessageType.Text when message.Text.Equals(BotConstants.Commands.CurrentAnnouncementsList) => 
                await GetCurrentAnnouncementListCommand(message),
            MessageType.Text when message.Text.Equals(BotConstants.Commands.WeatherCommand) => 
                await GetWeatherCommand(message),
            MessageType.Text when message.Text.Equals(BotConstants.Commands.GetSubscriptionsCommand) => 
                await GetSubscriptionsCommand(message),
            MessageType.Text when message.Text.Equals(BotConstants.Commands.EditSubscriptions) || 
                                  message.Text.Equals(BotConstants.Commands.EditWeatherSubscriptions) =>
                await GetEditSubscriptionsCommand(message),
            MessageType.Text when message.Text.Equals(BotConstants.Commands.FuelInfo) => 
                await GetFuelInfoCommand(message),
            _ => await UserCommandHelper.SearchCommand(_service, message)
        };
    }
    
    private async Task<IBaseRequest> GetFuelInfoCommand(TelegramMessageDto message)
    {
        await RemoveCache(message);
        return new GetFuelInfoCommand
        {
            ChatId = message.ChatId.ToString()
        };
    }
    
    private async Task<IBaseRequest> GetEditSubscriptionsCommand(TelegramMessageDto message)
    {
        await RemoveCache(message);
        return new EditSubscriptionsCommand
        {
            ExternalUserId = message.ChatId.ToString(),
            Text = message.Text
        };
    }
    
    private async Task<IBaseRequest> GetSubscriptionsCommand(TelegramMessageDto messageDto)
    {
        await RemoveCache(messageDto);
        return new GetSubscriptionsCommand
        {
            ExternalUserId = messageDto.ChatId.ToString()
        };
    }
    
    private async Task<IBaseRequest> GetWeatherCommand(TelegramMessageDto messageDto)
    {
        await RemoveCache(messageDto);
        return new GetWeatherCommand
        {
            ChatId = messageDto.ChatId.ToString()
        };
    }
    
    private async Task<IBaseRequest> GetCurrentAnnouncementListCommand(TelegramMessageDto messageDto)
    {
        var lastCommand = await _service
            .GetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{messageDto.ChatId}",
                () => Task.FromResult(new CacheModel()));
        
        return new MyAnnouncementsCommand
        {
            ChatId = messageDto.ChatId.ToString(),
            PageIndex = lastCommand.Announcement.PageIndex!.Value,
            MessageId = lastCommand.Announcement.MessageId
        };
    }
    
    private async Task<IBaseRequest> GetPreviousAnnouncementListCommand(TelegramMessageDto messageDto)
    {
        var lastCommand = await _service
            .GetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{messageDto.ChatId}",
                () => Task.FromResult(new CacheModel()));
        
        return new MyAnnouncementsCommand
        {
            ChatId = messageDto.ChatId.ToString(),
            PageIndex = lastCommand.Announcement.PageIndex!.Value - 1,
            MessageId = lastCommand.Announcement.MessageId
        };
    }
    
    private async Task<IBaseRequest> GetNextAnnouncementListCommand(TelegramMessageDto messageDto)
    {
        var lastCommand = await _service
            .GetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{messageDto.ChatId}",
                () => Task.FromResult(new CacheModel()));
        
        return new MyAnnouncementsCommand
        {
            ChatId = messageDto.ChatId.ToString(),
            PageIndex = lastCommand.Announcement.PageIndex!.Value + 1,
            MessageId = lastCommand.Announcement.MessageId
        };
    }

    private async Task RemoveCache(TelegramMessageDto messageDto)
    {
        await _service.DeleteUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{messageDto.ChatId}");
    }

    private async Task<IBaseRequest> GetSupportCommand(TelegramMessageDto messageDto)
    {
        await RemoveCache(messageDto);
        return new SupportCommand
        {
            ChatId = messageDto.ChatId.ToString()
        };
    }

    private async Task<IBaseRequest> GetMyAnnouncementsCommand(TelegramMessageDto message)
    {
        await RemoveCache(message);
        return new MyAnnouncementsCommand
        {
            ChatId = message.ChatId.ToString(),
            PageIndex = 1
        };
    }
    
    private async Task<IRequest> GetCancelAnnouncementCommand(TelegramMessageDto message)
    {
        var lastCommand = await _service
            .GetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{message.ChatId}",
                () => Task.FromResult(new CacheModel()));
        
        return new CancelAnnouncementCommand
        {
            ChatId = message.ChatId.ToString(),
            Id = lastCommand.Announcement.Id
        };
    }
    
    private async Task<IRequest> GetPostAnnouncementCommand(TelegramMessageDto message)
    {
        var lastCommand = await _service
            .GetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{message.ChatId}",
                () => Task.FromResult(new CacheModel()));
        
        return new PostAnnouncementCommand
        {
            ChatId = message.ChatId.ToString(),
            AnnouncementId = lastCommand.Announcement.Id
        };
    }
    
    private async Task<IRequest> GetSellCarCommand(TelegramMessageDto message)
    {
        await RemoveCache(message);
        return new CreateAnnouncement
        {
           ChatId = message.ChatId.ToString()
        };
    }
    
    private async Task<IRequest> GetRemoveGroupCommand(TelegramMessageDto message)
    {
        await RemoveCache(message);
        return new RemoveGroupCommand
        {
            GroupId = message.ChatId,
            UserId = message.UserDto.UserId
        };
    }

    private async Task<IRequest> GetAddGroupCommand(TelegramMessageDto message)
    {
        await RemoveCache(message);
        return new AddGroupCommand
        {
            GroupId = message.ChatId,
            UserId = message.UserDto.UserId
        };
    }
    
    private async Task<IRequest> GetRegisterUserCommand(TelegramMessageDto message)
    {
        await RemoveCache(message);
        return new StartCommand
        {
            ChatId = message.ChatId
        };
    }
    
    private async Task<IRequest> GetContactConfirmedCommand(TelegramMessageDto message)
    {
        await RemoveCache(message);
        return new SharedPhoneCommand
        {
            ChatId = message.ChatId,
            Phone = message.Text!,
            FullName = $"{message.UserDto.FirstName} {message.UserDto.LastName}",
            NickName = $"@{message.UserDto.NickName}"
        };
    }

    private static TelegramMessageDto GetMessageDto(Update update, ITelegramBotClient botClient)
    {
        var messageDto = new TelegramMessageDto();
        
        if ((update.Message?.Type)?.ToString() == MessageType.Text.ToString() ||
            (update.CallbackQuery?.Message?.Type)?.ToString() == MessageType.Text.ToString())
        {
            messageDto = new TelegramMessageDto
            {
                ChatId = update.CallbackQuery != null
                    ? update.CallbackQuery.Message!.Chat.Id
                    : update.Message!.Chat.Id,

                Text = update.CallbackQuery != null
                    ? update.CallbackQuery.Data
                    : update.Message!.Text,
                UserDto = new TelegramUserDto
                {
                    FirstName = update.CallbackQuery != null
                        ? update.CallbackQuery.From.FirstName
                        : update.Message!.From!.FirstName,
                    LastName = update.CallbackQuery != null
                        ? update.CallbackQuery.From.LastName
                        : update.Message!.From!.LastName,
                    NickName = update.CallbackQuery != null
                        ? update.CallbackQuery.From.Username
                        : update.Message!.From!.Username
                },
                Type = (update.CallbackQuery != null
                    ? update.CallbackQuery.Message!.Type
                    : update.Message!.Type)!,
            };
        }
        else if(update.Message?.Type == MessageType.Contact)
        {
            messageDto = new TelegramMessageDto
            {
                ChatId = update.Message!.Chat.Id,
                Text = "+" + update.Message.Contact?.PhoneNumber,
                UserDto = new TelegramUserDto
                {
                    FirstName = update.Message.Contact?.FirstName,
                    LastName = update.Message.Contact?.LastName,
                    NickName = $"@{update.Message.From?.Username}"
                },
                Type = update.Message.Type
            };
        }
        else if(update.Type == UpdateType.MyChatMember)
        {
            messageDto = new TelegramMessageDto
            {
                ChatId = update.MyChatMember!.Chat.Id,
                UserDto = new TelegramUserDto
                {
                    UserId = update.MyChatMember!.NewChatMember.User.Id
                },
                Type = update.MyChatMember!.NewChatMember.Status is ChatMemberStatus.Left or ChatMemberStatus.Kicked ?
                    MessageType.ChatMemberLeft : MessageType.ChatMembersAdded,
                Text = "bot is processed"
            };
        }
        else if(update.Message?.Type is MessageType.Photo or MessageType.Document)
        {
            messageDto = new TelegramMessageDto
            {
                ChatId = update.Message.Chat.Id,

                Text = "file",
                UserDto = new TelegramUserDto
                {
                    FirstName = update.Message.From!.FirstName,
                    LastName = update.Message.From!.LastName,
                    NickName = update.Message.From!.Username
                },
                Type = update.Message!.Type,
                FileId = update.Message?.Type == MessageType.Photo ? 
                    update.Message.Photo?[^1].FileId :
                    update.Message?.Document?.FileId
            };
        }
        else
            messageDto = update.Message?.Type switch
            {
                MessageType.ChatMembersAdded => new TelegramMessageDto
                {
                    ChatId = update.Message.Chat.Id,
                    UserDto = new TelegramUserDto
                    {
                        UserId = update.Message.NewChatMembers!.FirstOrDefault(e =>
                            e.IsBot && e.Id == botClient.BotId)!.Id
                    },
                    Type = update.Message.Type,
                    Text = "bot is processed"
                },
                MessageType.ChatMemberLeft => new TelegramMessageDto
                {
                    ChatId = update.Message.Chat.Id,
                    UserDto = new TelegramUserDto {UserId = update.Message.LeftChatMember!.Id},
                    Type = update.Message.Type,
                    Text = "bot is processed"
                },
                _ => messageDto
            };

        return messageDto;
    }
}