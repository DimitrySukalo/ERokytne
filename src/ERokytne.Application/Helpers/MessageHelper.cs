using System.Text;
using ERokytne.Application.Ports.Adapters.Weather.Responses;

namespace ERokytne.Application.Helpers;

public static class MessageHelper
{
    public static string GetRegistrationMessage()
    {
        var message = new StringBuilder();

        message.Append("⚡️Цей бот створений для того, щоб люди могли швидко отримувати потрібну їм інформацію.\n" +
                       "Він має 3 розділи:\n");
        message.Append(" \n");
        message.Append(
            "📑 Оголошення:\n- Тут ви можете подати своє оголошення на вашу тему.\n" +
            "Його будуть бачити всі учасники каналу.\nЯк створити оголошення? Дуже просто!\n");
        message.Append(" \n");
        message.Append("<b>Створити оголошення -> Вводите текст і відправляєте його -> " +
                       "По бажанню завантажуєте фото ( будь-яка кількість) -> Опублікувати</b> ✅\n");
        message.Append(" \n");
        message.Append("💻 Мої оголошення:\n");
        message.Append("- Тут ви можете переглядати та видаляти свої оголошення.\nЯк видалити оголошення?\n");
        message.Append(" \n");
        message.Append("<b>Мої оголошення -> Обираєте ваше оголошення -> Видалити оголошення</b> ✅\n");
        message.Append(" \n");
        message.Append("🙌 Підтримка\n");
        message.Append("- Тут ви можете написати своє запитання або запропонувати ідею, як покращити бота. " +
                       "Якщо ідея буде дійсно крута, то отримаєте від нас бонус!\nЯк відправити повідомлення в підтримку?");
        message.Append(" \n");
        message.Append("<b>Підтримка -> Вводите текст та відправляєте -> Готово</b> ✅\n");
        message.Append(" \n");
        message.Append("Бажаємо вам приємного користування! Ваш функціонал нижче 👇");

        return message.ToString();
    }

    public static string GetWeatherMessage(Forecastday weatherInfo, string? date)
    {
        var message = new StringBuilder();

        message.Append($"<b>Середні показники</b>\n");
        message.Append($"🌡️ Середня температура: {Math.Ceiling(weatherInfo.Day.AvgTemp)} °C\n");
        message.Append($"🌡️ Мінімальна температура: {Math.Ceiling(weatherInfo.Day.MinTemp)} °C\n");
        message.Append($"🌡️ Максимальна температура: {Math.Ceiling(weatherInfo.Day.MaxTemp)} °C\n");
        message.Append($"🌧 Шанс, що буде дощ: {weatherInfo.Day.DailyChanceOfRain} %\n");
        message.Append(" \n");

        var weatherHours = weatherInfo.Hours.Where(e => e.Time.Equals(date + " 00:00") ||
                                                   e.Time.Equals(date + " 03:00")
                                                   || e.Time.Equals(date + " 06:00")
                                                   || e.Time.Equals(date + " 09:00")
                                                   || e.Time.Equals(date + " 12:00")
                                                   || e.Time.Equals(date + " 15:00")
                                                   || e.Time.Equals(date + " 18:00")
                                                   || e.Time.Equals(date + " 21:00")).ToList();
        
        foreach (var weather in weatherHours)
        {
            message.Append($"<b>{weather.Time.Replace(date!, string.Empty).Trim()}</b>\n");
            message.Append($"🌡️ Температура: {Math.Ceiling(weather.Temp)} °C\n");
            message.Append($"🌧 Шанс дощу: {weather.ChanceOfRain} %\n");
            message.Append($"💨 Швидкість вітру: {Math.Round(weather.WindKph, 1)} км/год\n");
            message.Append(" \n");
        }
        
        return message.ToString();
    }
}