using System;
using System.Collections.Generic;
using System.Linq;
using ERokytne.Application.Ports.Adapters.Fuel;
using ERokytne.Domain.Entities;
using ERokytne.Persistence;
using ERokytne.Tests.Mocks;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace ERokytne.Tests.Helpers;

public static class MockHelper
{
    public static (Mock<TelegramBotMock> telegramBot, Mock<ApplicationDbContext> dbContext, Mock<IFuelAdapter> fuelAdapter) 
        GetMocks()
    {
        var telegramBot = new Mock<TelegramBotMock>();
        var dbContext = new Mock<ApplicationDbContext>();
        var fuelAdapter = new Mock<IFuelAdapter>();

        fuelAdapter.Setup(e => e.GetFuelInfo()).ReturnsAsync("Test data");
        dbContext.Setup(e => e.TelegramUsers).Returns(GetUsers().Object);

        return (telegramBot, dbContext, fuelAdapter);
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