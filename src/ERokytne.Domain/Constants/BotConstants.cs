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
        public const string SellCommand = "Продати свою річ 📑";
        public const string AnnouncementEnteredText = "/announcemententeredtext";
        public const string PostAnnouncement = "Опублікувати оголошення 📨";
        public const string CancelAnnouncement = "Відмінити оголошення ❌";
    }
}