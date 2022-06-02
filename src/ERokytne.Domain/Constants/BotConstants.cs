namespace ERokytne.Domain.Constants;

public static class BotConstants
{
    public static class Cache
    {
        public const string PreviousCommand = $"bot-api:{nameof(PreviousCommand)}";
    }
    
    public static class Commands
    {
        public const string StartCommand = "/start";
        public const string SellCommand = "📑 Зробити оголошення";
        public const string SupportCommand = "🙌 Підтримка";
        public const string NextAnnouncementsList = "/nextannouncementslist";
        public const string PreviousAnnouncementsList = "/previousannouncementslist";
        public const string CurrentAnnouncementsList = "/currentannouncementslist";
        public const string OpenAnnouncementCommand = "/openannouncementcommand";
        public const string MyAnnouncementsCommand = "💻 Мої оголошення";
        public const string WeatherCommand = "🌤 Погода";
        public const string AnnouncementEnteredText = "/announcemententeredtext";
        public const string PostAnnouncement = "📨 Опублікувати оголошення";
        public const string CancelAnnouncement = "❌ Відмінити оголошення";
        public const string DeleteAnnouncement = "❌ Видалити оголошення";
        public const string WeatherIsSelected = "/weatherisselected";
        public const string ReturnWeatherDayList = "/returnweatherdaylist";
    }
    
    public static class Messages
    {
        public static class Start
        {
            public const string SharePhoneNumberMessage = $"{nameof(Start)}:{nameof(SharePhoneNumberMessage)}";
            public const string ConfirmPhoneByButtonMessage = $"{nameof(Start)}:{nameof(ConfirmPhoneByButtonMessage)}";
            public const string MainMenuMessage = $"{nameof(Start)}:{nameof(MainMenuMessage)}";
            public const string UserIsRegisteredMessage = $"{nameof(Start)}:{nameof(UserIsRegisteredMessage)}";
        }
        
        public static class Support
        {
            public const string TipMessage = $"{nameof(Support)}:{nameof(TipMessage)}";
            public const string LimitMessage = $"{nameof(Support)}:{nameof(LimitMessage)}";
            public const string TipSavedMessage = $"{nameof(Support)}:{nameof(TipSavedMessage)}";
        }
        
        public static class Weather
        {
            public const string SelectDayMessage = $"{nameof(Weather)}:{nameof(SelectDayMessage)}";
            public const string DataNotFoundMessage = $"{nameof(Weather)}:{nameof(DataNotFoundMessage)}";
            public const string BackToDayListMessage = $"{nameof(Weather)}:{nameof(BackToDayListMessage)}";
        }
        
        public static class Announcement
        {
            public const string LimitMessage = $"{nameof(Announcement)}:{nameof(LimitMessage)}";
            public const string NickNameIsNotExistMessage =
                $"{nameof(Announcement)}:{nameof(NickNameIsNotExistMessage)}";
            public const string TextMessage = $"{nameof(Announcement)}:{nameof(TextMessage)}";
            public const string IsCanceledMessage = $"{nameof(Announcement)}:{nameof(IsCanceledMessage)}";
            public const string IsDeletedMessage = $"{nameof(Announcement)}:{nameof(IsDeletedMessage)}";
            public const string BackToListMessage = $"{nameof(Announcement)}:{nameof(BackToListMessage)}";
            public const string NextPageMessage = $"{nameof(Announcement)}:{nameof(NextPageMessage)}";
            public const string PreviousPageMessage = $"{nameof(Announcement)}:{nameof(PreviousPageMessage)}";
            public const string IsNotExistMessage = $"{nameof(Announcement)}:{nameof(IsNotExistMessage)}";
            public const string SelectMessage = $"{nameof(Announcement)}:{nameof(SelectMessage)}";
            public const string LimitTextLengthMessage = $"{nameof(Announcement)}:{nameof(LimitTextLengthMessage)}";
            public const string TextIsSavedMessage = $"{nameof(Announcement)}:{nameof(TextIsSavedMessage)}";
            public const string SendOnlyPhotosMessage = $"{nameof(Announcement)}:{nameof(SendOnlyPhotosMessage)}";
            public const string SelectActionMessage = $"{nameof(Announcement)}:{nameof(SelectActionMessage)}";
            public const string IsCreatedMessage = $"{nameof(Announcement)}:{nameof(IsCreatedMessage)}";
        }

        public static class NewDay
        {
            public const string HelloMessage = $"{nameof(NewDay)}:{nameof(HelloMessage)}";
        }
        
        public static class NotFound
        {
            public const string CommandIsNotFoundMessage = $"{nameof(NotFound)}:{nameof(CommandIsNotFoundMessage)}";
        }
    }
}