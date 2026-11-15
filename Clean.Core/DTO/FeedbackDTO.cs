namespace Clean.Core.DTO
{
    public class FeedbackDTO
    {
        public int Id { get; set; }
        public int BookingCode { get; set; }
        public int CleanerId { get; set; }
        public int Rating { get; set; }
        public string? TextOfFeedback { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
