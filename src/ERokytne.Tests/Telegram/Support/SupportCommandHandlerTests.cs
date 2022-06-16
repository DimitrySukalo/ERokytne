using System;
using System.Threading;
using System.Threading.Tasks;
using EasyCaching.Core;
using ERokytne.Application.Cache;
using ERokytne.Application.Telegram.Commands.Support.Commands;
using ERokytne.Application.Telegram.Models;
using ERokytne.Tests.Helpers;
using Moq;
using Xunit;

namespace ERokytne.Tests.Telegram.Support;

public static class SupportCommandHandlerTests
{
    [Fact]
    private static async Task HandlerMethodThrowsException()
    {
        //Arrange
        var (telegramBot, dbContext, _) = MockHelper.GetMocks();
        var hybridCache = new Mock<IHybridCachingProvider>();
        var actionsService = new Mock<UserActionService>(hybridCache.Object);

        actionsService.Setup(e => 
            e.SetUserCacheAsync(It.IsAny<string>(), It.IsAny<CacheModel>())).Returns(Task.CompletedTask);

        var commandHandler = new SupportCommandHandler(telegramBot.Object, dbContext.Object, actionsService.Object);
        
        //Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await 
            commandHandler.Handle(new SupportCommand
        {
            ChatId = "2"
        }, CancellationToken.None));
    }
    
    [Fact]
    private static void HandlerMethodReturnsSuccessful()
    {
        //Arrange
        var (telegramBot, dbContext, _) = MockHelper.GetMocks();
        var hybridCache = new Mock<IHybridCachingProvider>();
        var actionsService = new Mock<UserActionService>(hybridCache.Object);

        actionsService.Setup(e => 
            e.SetUserCacheAsync(It.IsAny<string>(), It.IsAny<CacheModel>())).Returns(Task.CompletedTask);

        var commandHandler = new SupportCommandHandler(telegramBot.Object, dbContext.Object, actionsService.Object);
        
        //Act
        bool response;
        try
        {
            commandHandler.Handle(new SupportCommand
            {
                ChatId = "1"
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