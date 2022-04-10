using ERokytne.Application.Cache;
using ERokytne.Application.Telegram.Commands;
using ERokytne.Application.Telegram.Commands.Announcements;
using ERokytne.Application.Telegram.Commands.Support.Commands;
using ERokytne.Application.Telegram.Models;
using ERokytne.Domain.Constants;
using MediatR;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ERokytne.Application.Helpers;

public static class UserCommandHelper
{
    public static async Task<IBaseRequest?> SearchCommand(UserActionService registrationService,
        TelegramMessageDto message)
    {
        var lastCommand = await registrationService
            .GetUserCacheAsync($"{BotConstants.Cache.PreviousCommand}:{message.ChatId}",
                () => Task.FromResult(new AnnouncementCacheModel()));

        if (!string.IsNullOrWhiteSpace(lastCommand.PreviousCommand))
        {
            return lastCommand.PreviousCommand switch
            {
                BotConstants.Commands.SellCommand => GetAddAnnouncementMessageCommand(message, lastCommand.Id),
                BotConstants.Commands.AnnouncementEnteredText when message.Type is MessageType.Photo or MessageType.Document
                    => GetAddAnnouncementPhotosCommand(message, lastCommand.Id),
                BotConstants.Commands.SupportCommand => GetCreateSupportMessageCommand(message),
                _ => null
            };
        }

        return new NotFoundCommand
        {
            ChatId = message.ChatId
        };
    }

    public static ReplyKeyboardMarkup GetStartMenu()
    {
        return new ReplyKeyboardMarkup(new List<IEnumerable<KeyboardButton>>
        {
            new List<KeyboardButton>
            {
                new(BotConstants.Commands.SellCommand),
                new(BotConstants.Commands.MyAnnouncementsCommand)
            },
            new List<KeyboardButton>
            {
                new(BotConstants.Commands.SupportCommand)
            }
        })
        {
            ResizeKeyboard = true
        };
    }

    private static IBaseRequest GetCreateSupportMessageCommand(TelegramMessageDto messageDto)
    {
        return new SaveSupportMessageCommand
        {
            ChatId = messageDto.ChatId.ToString(),
            Text = messageDto.Text
        };
    }

    private static IBaseRequest GetAddAnnouncementPhotosCommand(TelegramMessageDto messageDto, Guid? announcementId)
    {
        return new AddAnnouncementPhotosCommand
        {
            ChatId = messageDto.ChatId.ToString(),
            AnnouncementId = announcementId,
            FileId = messageDto.FileId,
            MessageType = messageDto.Type
        };
    }
    
    private static IBaseRequest GetAddAnnouncementMessageCommand(TelegramMessageDto messageDto, Guid? announcementId)
    {
        return new AddAnnouncementMessageCommand
        {
            ChatId = messageDto.ChatId.ToString(),
            Text = messageDto.Text,
            AnnouncementId = announcementId
        };
    }
}