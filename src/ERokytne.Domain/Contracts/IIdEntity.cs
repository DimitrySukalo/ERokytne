namespace ERokytne.Domain.Contracts
{
    public interface IIdEntity<T>
    {
        public T Id { get; set; }
    }
}