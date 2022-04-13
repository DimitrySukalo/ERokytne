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
        public static class StartCommand
        {
            public const string SharePhoneNumber = $"{nameof(StartCommand)}:{nameof(SharePhoneNumber)}";
            public const string ConfirmPhoneByButton = $"{nameof(StartCommand)}:{nameof(ConfirmPhoneByButton)}";
            public const string MainMenu = $"{nameof(StartCommand)}:{nameof(MainMenu)}";
        }
    }
}