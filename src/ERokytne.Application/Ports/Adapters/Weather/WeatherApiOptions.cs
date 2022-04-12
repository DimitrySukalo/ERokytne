namespace ERokytne.Application.Ports.Adapters.Weather;

public class WeatherApiOptions
{
    public string? Url { get; set; }
    
    public string? ApiKey { get; set; }
    
    public string? City { get; set; }
}