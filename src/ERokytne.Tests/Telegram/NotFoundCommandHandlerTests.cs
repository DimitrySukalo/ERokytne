using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERokytne.Application.Telegram.Commands;
using ERokytne.Domain.Entities;
using ERokytne.Persistence;
using ERokytne.Tests.Mocks;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace ERokytne.Tests.Telegram;

public static class NotFoundCommandHandlerTests
{
    [Fact]
    private static void NotFoundCommandHandlerReturnSuccessful()
    {
        //Arrange
        var telegramBot = new Mock<TelegramBotMock>();
        var dbContext = new Mock<ApplicationDbContext>();
        dbContext.Setup(e => e.TelegramUsers).Returns(GetUsers().Object);
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
        var telegramBot = new Mock<TelegramBotMock>();
        var dbContext = new Mock<ApplicationDbContext>();
        dbContext.Setup(e => e.TelegramUsers).Returns(GetUsers().Object);
        var notFoundCommandHandler = new NotFoundCommandHandler(telegramBot.Object, dbContext.Object);

        //Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            notFoundCommandHandler.Handle(new NotFoundCommand
        {
            ChatId = 2
        }, CancellationToken.None));
        
    }

    private static Mock<DbSet<TelegramUser>> GetUsers()
    {
        var users = new List<TelegramUser>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ChatId = "1"
            }
        }.AsQueryable().BuildMockDbSet();

        return users;
    }

}