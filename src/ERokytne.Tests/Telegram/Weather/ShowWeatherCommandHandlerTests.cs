using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EasyCaching.Core;
using ERokytne.Application.Cache;
using ERokytne.Application.Ports.Adapters.Weather;
using ERokytne.Application.Telegram.Commands.Weather;
using ERokytne.Application.Telegram.Models;
using ERokytne.Infrastructure.Adapters.WeatherApi;
using ERokytne.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ERokytne.Tests.Telegram.Weather;

public static class ShowWeatherCommandHandlerTests
{
    [Fact]
    private static void ShowWeatherCommandHandlerThrowsException()
    {
        //Arrange
        var (telegramBot, dbContext, _) = MockHelper.GetMocks();
        var hybridCache = new Mock<IHybridCachingProvider>();
        var actionsService = new Mock<UserActionService>(hybridCache.Object);

        actionsService.Setup(e => 
            e.SetUserCacheAsync(It.IsAny<string>(), It.IsAny<CacheModel>())).Returns(Task.CompletedTask);
        
        var logger = new Mock<ILogger<WeatherApiAdapter>>();
        var optionsFactory = new Mock<IOptionsFactory<WeatherApiOptions>>();
        
        var options = new Mock<OptionsManager<WeatherApiOptions>>(optionsFactory.Object);
        
        options.Setup(e => e.Get(It.IsAny<string>())).Returns(new WeatherApiOptions
        {
            Url = "https://api.weatherapi.com/v1/forecast.json",
            ApiKey = "95617e412ae645d3820202344221004",
            City = "Rokytne"
        });

        var day = DateTime.UtcNow.ToString("yyyy-MM-dd");
        
        var adapter = new WeatherApiAdapter(new HttpClient(), logger.Object, options.Object);
        var commandHandler = new ShowWeatherCommandHandler(telegramBot.Object, dbContext.Object, 
            adapter, actionsService.Object);
        
        //Act and Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => commandHandler.Handle(new ShowWeatherCommand
        {
            ChatId = "2",
            Day = day,
            MessageId = 1
        }, CancellationToken.None));
    }
    
    [Fact]
    private static void ShowWeatherCommandHandlerReturnsSuccessful()
    {
        //Arrange
        var (telegramBot, dbContext, _) = MockHelper.GetMocks();
        var hybridCache = new Mock<IHybridCachingProvider>();
        var actionsService = new Mock<UserActionService>(hybridCache.Object);

        actionsService.Setup(e => 
            e.SetUserCacheAsync(It.IsAny<string>(), It.IsAny<CacheModel>())).Returns(Task.CompletedTask);
        
        var logger = new Mock<ILogger<WeatherApiAdapter>>();
        var optionsFactory = new Mock<IOptionsFactory<WeatherApiOptions>>();
        
        var options = new Mock<OptionsManager<WeatherApiOptions>>(optionsFactory.Object);
        
        options.Setup(e => e.Get(It.IsAny<string>())).Returns(new WeatherApiOptions
        {
            Url = "https://api.weatherapi.com/v1/forecast.json",
            ApiKey = "95617e412ae645d3820202344221004",
            City = "Rokytne"
        });

        var day = DateTime.UtcNow.ToString("yyyy-MM-dd");
        
        var adapter = new WeatherApiAdapter(new HttpClient(), logger.Object, options.Object);
        var commandHandler = new ShowWeatherCommandHandler(telegramBot.Object, dbContext.Object, 
            adapter, actionsService.Object);
        
        //Act
        bool result;
        try
        {
            commandHandler.Handle(new ShowWeatherCommand
            {
                ChatId = "1",
                Day = day,
                MessageId = 1
            }, CancellationToken.None);
            result = true;
        }
        catch (Exception)
        {
            result = false;
        }
        
        //Assert
        Assert.True(result);
    }
}