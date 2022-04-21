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
            public const string SharePhoneNumber = $"{nameof(Start)}:{nameof(SharePhoneNumber)}";
            public const string ConfirmPhoneByButton = $"{nameof(Start)}:{nameof(ConfirmPhoneByButton)}";
            public const string MainMenu = $"{nameof(Start)}:{nameof(MainMenu)}";
        }
        
        public static class Support
        {
            public const string TipMessage = $"{nameof(Support)}:{nameof(TipMessage)}";
            public const string LimitMessage = $"{nameof(Support)}:{nameof(LimitMessage)}";
            public const string TipSavedMessage = $"{nameof(Support)}:{nameof(TipSavedMessage)}";
        }
    }
}