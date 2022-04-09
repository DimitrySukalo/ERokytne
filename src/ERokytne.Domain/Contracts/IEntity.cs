namespace ERokytne.Domain.Contracts
{
    public interface IEntity<T> : IIdEntity<T>, ITrackEntity
    {
    }
}