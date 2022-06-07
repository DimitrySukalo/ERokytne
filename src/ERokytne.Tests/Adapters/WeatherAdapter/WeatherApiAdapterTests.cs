using System;
using System.Net.Http;
using System.Threading.Tasks;
using ERokytne.Application.Ports.Adapters.Weather;
using ERokytne.Infrastructure.Adapters.WeatherApi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ERokytne.Tests.Adapters.WeatherAdapter;

public static class WeatherApiAdapterTests
{
    [Fact]
    private static async Task GetWeatherInfoThrowsException()
    {
        //Arrange
        var logger = new Mock<ILogger<WeatherApiAdapter>>();
        var optionsFactory = new Mock<IOptionsFactory<WeatherApiOptions>>();
        
        var options = new Mock<OptionsManager<WeatherApiOptions>>(optionsFactory.Object);
        
        options.Setup(e => e.Get(It.IsAny<string>())).Returns(new WeatherApiOptions
        {
            Url = "https://api.weatherapi.com/v1/forecast.json",
            ApiKey = "1",
            City = "Rokytne"
        });

        var adapter = new WeatherApiAdapter(new HttpClient(), logger.Object, options.Object);

        //Act and Assert
        await Assert.ThrowsAnyAsync<Exception>(async () => await adapter.GetWeatherInfo(1));
    }
    
    [Fact]
    private static async Task GetWeatherInfoReturnsOneDayInfo()
    {
        //Arrange
        var logger = new Mock<ILogger<WeatherApiAdapter>>();
        var optionsFactory = new Mock<IOptionsFactory<WeatherApiOptions>>();
        
        var options = new Mock<OptionsManager<WeatherApiOptions>>(optionsFactory.Object);
        
        options.Setup(e => e.Get(It.IsAny<string>())).Returns(new WeatherApiOptions
        {
            Url = "https://api.weatherapi.com/v1/forecast.json",
            ApiKey = "95617e412ae645d3820202344221004",
            City = "Rokytne"
        });

        var adapter = new WeatherApiAdapter(new HttpClient(), logger.Object, options.Object);

        //Act
        var response = await adapter.GetWeatherInfo(1);
        
        //Assert
        Assert.NotNull(response);
        Assert.NotNull(response?.Forecast);
        Assert.NotNull(response?.Forecast.ForecastDays);
        Assert.InRange((int)response?.Forecast.ForecastDays.Count!, 1, 1);
    }
    
    [Fact]
    private static async Task GetWeatherInfoReturnsTwoDayInfo()
    {
        //Arrange
        var logger = new Mock<ILogger<WeatherApiAdapter>>();
        var optionsFactory = new Mock<IOptionsFactory<WeatherApiOptions>>();
        
        var options = new Mock<OptionsManager<WeatherApiOptions>>(optionsFactory.Object);
        
        options.Setup(e => e.Get(It.IsAny<string>())).Returns(new WeatherApiOptions
        {
            Url = "https://api.weatherapi.com/v1/forecast.json",
            ApiKey = "95617e412ae645d3820202344221004",
            City = "Rokytne"
        });

        var adapter = new WeatherApiAdapter(new HttpClient(), logger.Object, options.Object);

        //Act
        var response = await adapter.GetWeatherInfo(2);
        
        //Assert
        Assert.NotNull(response);
        Assert.NotNull(response?.Forecast);
        Assert.NotNull(response?.Forecast.ForecastDays);
        Assert.InRange((int)response?.Forecast.ForecastDays.Count!, 2, 2);
    }
    
    [Fact]
    private static async Task GetWeatherInfoReturnsThreeDayInfo()
    {
        //Arrange
        var logger = new Mock<ILogger<WeatherApiAdapter>>();
        var optionsFactory = new Mock<IOptionsFactory<WeatherApiOptions>>();
        
        var options = new Mock<OptionsManager<WeatherApiOptions>>(optionsFactory.Object);
        
        options.Setup(e => e.Get(It.IsAny<string>())).Returns(new WeatherApiOptions
        {
            Url = "https://api.weatherapi.com/v1/forecast.json",
            ApiKey = "95617e412ae645d3820202344221004",
            City = "Rokytne"
        });

        var adapter = new WeatherApiAdapter(new HttpClient(), logger.Object, options.Object);

        //Act
        var response = await adapter.GetWeatherInfo(3);
        
        //Assert
        Assert.NotNull(response);
        Assert.NotNull(response?.Forecast);
        Assert.NotNull(response?.Forecast.ForecastDays);
        Assert.InRange((int)response?.Forecast.ForecastDays.Count!, 3, 3);
    }
}