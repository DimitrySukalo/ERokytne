using System;
using System.Collections.Generic;
using System.Linq;
using ERokytne.Domain.Entities;
using ERokytne.Persistence;
using ERokytne.Tests.Mocks;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace ERokytne.Tests.Helpers;

public static class MockHelper
{
    public static (Mock<TelegramBotMock> telegramBot, Mock<ApplicationDbContext> dbContext) GetMocks()
    {
        var telegramBot = new Mock<TelegramBotMock>();
        var dbContext = new Mock<ApplicationDbContext>();
        dbContext.Setup(e => e.TelegramUsers).Returns(GetUsers().Object);

        return (telegramBot, dbContext);
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