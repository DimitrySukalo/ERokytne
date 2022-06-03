using ERokytne.Application.Cache;
using ERokytne.Application.Telegram.Commands;
using ERokytne.Application.Telegram.Commands.Announcements;
using ERokytne.Application.Telegram.Commands.Support.Commands;
using ERokytne.Application.Telegram.Commands.Weather;
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
                () => Task.FromResult(new CacheModel()));

        if (!string.IsNullOrWhiteSpace(lastCommand.PreviousCommand))
        {
            return lastCommand.PreviousCommand switch
            {
                BotConstants.Commands.SellCommand => GetAddAnnouncementMessageCommand(message, lastCommand.Announcement.Id),
                BotConstants.Commands.AnnouncementEnteredText when message.Type is MessageType.Photo or MessageType.Document
                    => GetAddAnnouncementPhotosCommand(message, lastCommand.Announcement.Id),
                BotConstants.Commands.SupportCommand => GetCreateSupportMessageCommand(message),
                BotConstants.Commands.MyAnnouncementsCommand when Guid.TryParse(message.Text, out _)
                    => GetOpenAnnouncementCommand(message, lastCommand.Announcement.MessageId),
                BotConstants.Commands.OpenAnnouncementCommand when Guid.TryParse(message.Text, out _) => 
                    GetDeleteAnnouncementCommand(message, lastCommand.Announcement.MessageId),
                BotConstants.Commands.WeatherCommand when DateTime.TryParse(message.Text, out _) =>
                    ShowWeatherCommand(message, lastCommand.Weather.MessageId, message.Text),
                BotConstants.Commands.WeatherIsSelected when message.Text!.Equals(BotConstants.Commands.ReturnWeatherDayList)
                     => GetWeatherListCommand(message, lastCommand.Weather.MessageId),
                _ => new NotFoundCommand
                {
                    ChatId = message.ChatId
                }
            };
        }
        
        return new NotFoundCommand
        {
            ChatId = message.ChatId
        };
    }

    private static IBaseRequest GetWeatherListCommand(TelegramMessageDto messageDto, int? messageId)
    {
        var id = int.Parse(messageId.ToString()!);
        return new GetWeatherCommand
        {
            ChatId = messageDto.ChatId.ToString(),
            MessageId = id
        };
    }
    
    private static IBaseRequest ShowWeatherCommand(TelegramMessageDto messageDto, int? messageId, string? day)
    {
        var id = int.Parse(messageId.ToString()!);
        return new ShowWeatherCommand
        {
            ChatId = messageDto.ChatId.ToString(),
            MessageId = id,
            Day = day
        };
    }
    
    private static IBaseRequest GetDeleteAnnouncementCommand(TelegramMessageDto messageDto, int? messageId)
    {
        var id = int.Parse(messageId.ToString()!);
        return new DeleteAnnouncementCommand
        {
            ChatId = messageDto.ChatId.ToString(),
            Id = Guid.Parse(messageDto.Text!),
            MessageId = id
        };
    }
    
    private static IBaseRequest GetOpenAnnouncementCommand(TelegramMessageDto messageDto, int? messageId)
    {
        var id = int.Parse(messageId.ToString()!);
        return new OpenAnnouncementCommand
        {
            ChatId = messageDto.ChatId.ToString(),
            Id = Guid.Parse(messageDto.Text!),
            MessageId = id
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
                new(BotConstants.Commands.SupportCommand),
                new(BotConstants.Commands.WeatherCommand)
            },
            new List<KeyboardButton>
            {
                new(BotConstants.Commands.GetSubscriptionsCommand),
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