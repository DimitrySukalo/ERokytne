namespace ERokytne.Application.Ports.Adapters.Fuel;

public interface IFuelAdapter
{
    public Task<string> GetFuelInfo();
}