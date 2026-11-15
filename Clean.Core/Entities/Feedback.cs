namespace Clean.Core.Entities
{
    public class Feedback
    {
        public int Id { get; set; }
        public int BookingCode { get; set; }
        public Bookings Booking { get; set; }
        public int CleanerId { get; set; }
        public Cleaners Cleaner { get; set; }
        public int Rating { get; set; }
        public string? TextOfFeedback { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
