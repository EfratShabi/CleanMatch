namespace Clean.Core.Entities
{
    public class BlockedDate
    {
        public int Id { get; set; }
        public int CleanerId { get; set; }
        public Cleaners Cleaner { get; set; }
        public DateOnly Date { get; set; }
    }
}
