using System;
using System.Threading;
using System.Threading.Tasks;
using EasyCaching.Core;
using ERokytne.Application.Cache;
using ERokytne.Application.Telegram.Commands.Weather;
using ERokytne.Application.Telegram.Models;
using ERokytne.Tests.Helpers;
using Moq;
using Xunit;

namespace ERokytne.Tests.Telegram.Weather;

public static class GetWeatherCommandHandlerTests
{
    [Fact]
    private static void GetWeatherCommandHandlerThrowsException()
    {
        //Arrange
        var (telegramBot, dbContext, _) = MockHelper.GetMocks();
        var hybridCache = new Mock<IHybridCachingProvider>();
        var actionsService = new Mock<UserActionService>(hybridCache.Object);

        actionsService.Setup(e => 
                e.SetUserCacheAsync(It.IsAny<string>(), It.IsAny<CacheModel>())).Returns(Task.CompletedTask);

        var getWeatherCommandHandler =
            new GetWeatherCommandHandler(telegramBot.Object, dbContext.Object, actionsService.Object);

        //Act and Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => getWeatherCommandHandler.Handle(new GetWeatherCommand
        {
            ChatId = "2",
            MessageId = 1
        }, CancellationToken.None));
    }
    
    [Fact]
    private static void GetWeatherCommandHandlerReturnsSuccessful()
    {
        //Arrange
        var (telegramBot, dbContext, _) = MockHelper.GetMocks();
        var hybridCache = new Mock<IHybridCachingProvider>();
        var actionsService = new Mock<UserActionService>(hybridCache.Object);

        actionsService.Setup(e => 
            e.SetUserCacheAsync(It.IsAny<string>(), It.IsAny<CacheModel>())).Returns(Task.CompletedTask);

        var getWeatherCommandHandler =
            new GetWeatherCommandHandler(telegramBot.Object, dbContext.Object, actionsService.Object);

        //Act
        bool response;
        try
        {
            getWeatherCommandHandler.Handle(new GetWeatherCommand
            {
                ChatId = "2",
                MessageId = 1
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