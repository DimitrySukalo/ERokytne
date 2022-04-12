using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using ERokytne.Application.Ports.Adapters.Weather;
using ERokytne.Application.Ports.Adapters.Weather.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ERokytne.Infrastructure.Adapters.WeatherApi;

public class WeatherApiAdapter : IWeatherApiAdapter
{
    private readonly HttpClient _httpClient;
    private readonly WeatherApiOptions _options;
    private readonly ILogger<WeatherApiAdapter> _logger;

    public WeatherApiAdapter(HttpClient httpClient, ILogger<WeatherApiAdapter> logger, IOptions<WeatherApiOptions> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<WeatherInfoResponse?> GetWeatherInfo(int countOfDays, bool showAirQuality = false, bool showAlert = false) =>
        await PerformRequest<WeatherInfoResponse>(async client =>
            await client.GetAsync($"{_options.Url}?key={_options.ApiKey}&q={_options.City}&days=" +
                                  $"{countOfDays}&aqi={showAirQuality}&alerts={showAlert}"));
    
    private async Task<TResponse?> PerformRequest<TResponse>(Func<HttpClient, Task<HttpResponseMessage>> request)
    {
        HttpResponseMessage response;
        try
        {
            response = await request.Invoke(_httpClient);
        }
        catch (Exception e)
        {
            throw new ValidationException($"Weather api server response error. Message: {e.Message}");
        }

        if (response.IsSuccessStatusCode)
        {
            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<TResponse>(stream);
        }
        
        var error = await response.Content.ReadAsStringAsync();
        
        _logger.LogInformation(
            "Error request to weather Api {@request}. Response code: {statusCode}. Response body: {response}.", 
            request, response.StatusCode, error);
        
        throw new ValidationException(
            $"Error request to weather Api. Response code: {response.StatusCode}. Response body: {error}.");
    }
}