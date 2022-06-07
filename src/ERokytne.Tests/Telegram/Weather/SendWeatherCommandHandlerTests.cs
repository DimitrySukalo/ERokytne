using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ERokytne.Application.Ports.Adapters.Weather;
using ERokytne.Application.Ports.Adapters.Weather.Responses;
using ERokytne.Application.Telegram.Commands.Weather;
using ERokytne.Infrastructure.Adapters.WeatherApi;
using ERokytne.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ERokytne.Tests.Telegram.Weather;

public static class SendWeatherCommandHandlerTests
{
    [Fact]
    private static async Task SendWeatherCommandHandlerThrowsException()
    {
        //Arrange
        var (telegramBot, _, _) = MockHelper.GetMocks();
        var adapter = new Mock<IWeatherApiAdapter>();
        adapter.Setup(e => e.GetWeatherInfo(It.IsAny<int>(), It.IsAny<bool>(), 
                It.IsAny<bool>()))
            .ReturnsAsync(new WeatherInfoResponse
            {
                Forecast = new Forecast
                {
                    ForecastDays = new List<Forecastday>()
                }
            });
        
        var commandHandler = new SendWeatherCommandHandler(adapter.Object, telegramBot.Object);
        
        //Act and Assert
        var response = await Assert.ThrowsAsync<ValidationException>(async () => await commandHandler.Handle(new SendWeatherCommand
        {
            Chats = new List<string?>()
        }, CancellationToken.None));
        
        Assert.Equal("Weather response is empty", response.Message);
    }
    
    [Fact]
    private static async Task SendWeatherCommandHandlerReturnsSuccessful()
    {
        //Arrange
        var (telegramBot, _, _) = MockHelper.GetMocks();
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
        
        var commandHandler = new SendWeatherCommandHandler(adapter, telegramBot.Object);
        
        //Act
        bool response;
        try
        {
            await commandHandler.Handle(new SendWeatherCommand
            {
                Chats = new List<string?>()
            }, CancellationToken.None);
            response = true;
        }
        catch (Exception)
        {
            response = false;
        }
        
        //Assert
        Assert.True(response);
    }
}