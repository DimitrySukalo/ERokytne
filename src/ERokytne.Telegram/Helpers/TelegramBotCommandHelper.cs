using ERokytne.Application.Telegram.Commands;
using ERokytne.Application.Telegram.Models;
using ERokytne.Telegram.Contracts;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ERokytne.Telegram.Helpers;

public class TelegramBotCommandHelper : ITelegramBotCommandHelper
{
    public IBaseRequest FindCommand(Update update, ITelegramBotClient botClient)
    {
        var message = GetMessageDto(update, botClient);
        
        if (string.IsNullOrWhiteSpace(message.Text))
        {
            return null!;
        }

        return message.Type switch
        {
            
            _ => GetNotFoundCommand(message)
        };
    }
    
    private static IRequest GetNotFoundCommand(TelegramMessageDto message)
    {
        return new NotFoundCommand
        {
            ChatId = message.ChatId
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
                        : update.Message!.From!.LastName
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