namespace Clean.Core.Models
{
    public class FeedbackPostModel
    {
        public int BookingCode { get; set; }
        public int Rating { get; set; }
        public string? TextOfFeedback { get; set; }
    }
}
