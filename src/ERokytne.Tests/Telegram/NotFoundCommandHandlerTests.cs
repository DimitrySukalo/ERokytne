using System;
using System.Threading;
using System.Threading.Tasks;
using ERokytne.Application.Telegram.Commands;
using ERokytne.Tests.Helpers;
using Xunit;

namespace ERokytne.Tests.Telegram;

public static class NotFoundCommandHandlerTests
{
    [Fact]
    private static void NotFoundCommandHandlerReturnSuccessful()
    {
        //Arrange
        var (telegramBot, dbContext, _) = MockHelper.GetMocks();
        var notFoundCommandHandler = new NotFoundCommandHandler(telegramBot.Object, dbContext.Object);

        //Act
        bool result;
        try
        {
            notFoundCommandHandler.Handle(new NotFoundCommand
            {
                ChatId = 1
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
    
    [Fact]
    private static async Task NotFoundCommandHandlerThrowsArgumentNullException()
    {
        //Arrange
        var (telegramBot, dbContext, _) = MockHelper.GetMocks();
        var notFoundCommandHandler = new NotFoundCommandHandler(telegramBot.Object, dbContext.Object);

        //Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            notFoundCommandHandler.Handle(new NotFoundCommand
        {
            ChatId = 2
        }, CancellationToken.None));
    }
}