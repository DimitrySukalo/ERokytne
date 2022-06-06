using System;
using System.Threading;
using ERokytne.Application.Telegram.Commands;
using ERokytne.Tests.Helpers;
using Xunit;

namespace ERokytne.Tests.Telegram;

public static class SomeErrorCommandHandlerTests
{
    [Fact]
    private static void SomeErrorCommandHandlerReturnSuccessful()
    {
        //Arrange
        var (telegramBot, _, _) = MockHelper.GetMocks();
        var someErrorCommandHandler = new SomeErrorCommandHandler(telegramBot.Object);

        //Act
        bool result;
        try
        {
            someErrorCommandHandler.Handle(new SomeErrorCommand
            {
                ChatId = "1"
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