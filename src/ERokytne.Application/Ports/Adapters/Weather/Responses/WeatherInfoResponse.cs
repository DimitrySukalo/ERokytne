using System.Text.Json.Serialization;

namespace ERokytne.Application.Ports.Adapters.Weather.Responses;

public class WeatherInfoResponse
{
    [JsonPropertyName("forecast")]
    public Forecast Forecast { get; set; }
}

public class Day
{
    [JsonPropertyName("maxtemp_c")]
    public double MaxTemp { get; set; }
    
    [JsonPropertyName("mintemp_c")]
    public double MinTemp { get; set; }
    
    [JsonPropertyName("avgtemp_c")]
    public double AvgTemp { get; set; }
    
    [JsonPropertyName("maxwind_kph")]
    public double MaxWind { get; set; }
    
    [JsonPropertyName("daily_will_it_rain")]
    public int DailyWillItRain { get; set; }
    
    [JsonPropertyName("daily_chance_of_rain")]
    public int DailyChanceOfRain { get; set; }
}

public class Astro
{
    [JsonPropertyName("sunrise")]
    public string Sunrise { get; set; }
    
    [JsonPropertyName("sunset")]
    public string Sunset { get; set; }
}

public class Hour
{
    [JsonPropertyName("time")]
    public string Time { get; set; }
    
    [JsonPropertyName("temp_c")]
    public double Temp { get; set; }
    
    [JsonPropertyName("wind_kph")]
    public double WindKph { get; set; }
    
    [JsonPropertyName("cloud")]
    public int Cloud { get; set; }
    
    [JsonPropertyName("will_it_rain")]
    public int WillItRain { get; set; }
    
    [JsonPropertyName("chance_of_rain")]
    public int ChanceOfRain { get; set; }
}

public class Forecastday
{
    [JsonPropertyName("date")]
    public string Date { get; set; }
    
    [JsonPropertyName("day")]
    public Day Day { get; set; }
    
    [JsonPropertyName("astro")]
    public Astro Astro { get; set; }
    
    [JsonPropertyName("hour")]
    public List<Hour> Hours { get; set; }
}

public class Forecast
{
    [JsonPropertyName("forecastday")]
    public List<Forecastday> ForecastDays { get; set; }
}