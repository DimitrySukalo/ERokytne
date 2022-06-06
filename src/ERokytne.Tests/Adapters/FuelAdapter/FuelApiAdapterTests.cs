using System.Threading.Tasks;
using ERokytne.Infrastructure.Adapters.FuelApi;
using Xunit;

namespace ERokytne.Tests.Adapters.FuelAdapter;

public static class FuelApiAdapterTests
{
    [Fact]
    private static async Task GetFuelInfoReturnsSuccessful()
    {
        //Arrange
        var adapter = new FuelApiAdapter();

        //Act
        var data = await adapter.GetFuelInfo();

        //Assert
        Assert.NotNull(data);
    }
}