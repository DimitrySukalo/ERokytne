using System;
using System.Threading;
using System.Threading.Tasks;
using ERokytne.Application.Telegram.Commands.Fuel;
using ERokytne.Tests.Helpers;
using Xunit;

namespace ERokytne.Tests.Telegram.Fuel;

public static class GetFuelInfoCommandHandlerTests
{
    [Fact]
    private static void GetFuelInfoCommandHandlerReturnSuccessful()
    {
        //Arrange
        var (telegramBot, dbContext, fuelAdapter) = 
            MockHelper.GetMocks();
        var getFuelInfoCommandHandler = new GetFuelInfoCommandHandler(fuelAdapter.Object, telegramBot.Object, dbContext.Object);

        //Act
        bool result;
        try
        {
            getFuelInfoCommandHandler.Handle(new GetFuelInfoCommand
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
    
    [Fact]
    private static async Task GetFuelInfoCommandHandlerThrowsArgumentNullException()
    {
        //Arrange
        var (telegramBot, dbContext, fuelAdapter) =
            MockHelper.GetMocks();
        var getFuelInfoCommandHandler = new GetFuelInfoCommandHandler(fuelAdapter.Object, telegramBot.Object, dbContext.Object);

        //Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            getFuelInfoCommandHandler.Handle(new GetFuelInfoCommand
            {
                ChatId = "2"
            }, CancellationToken.None));
    }
}