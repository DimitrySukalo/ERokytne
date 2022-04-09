namespace ERokytne.Domain.Contracts
{
    public interface ITrackEntity
    {
        public DateTime CreatedOn { get; set; }
        
        public DateTime? UpdatedOn { get; set; }
    }
}