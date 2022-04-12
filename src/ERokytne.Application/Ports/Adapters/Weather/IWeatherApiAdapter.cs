using ERokytne.Application.Ports.Adapters.Weather.Responses;

namespace ERokytne.Application.Ports.Adapters.Weather;

public interface IWeatherApiAdapter
{
    public Task<WeatherInfoResponse?> GetWeatherInfo(int countOfDays, bool showAirQuality, bool showAlert);
}