namespace CleanAPI.Models
{
    public class CleanerAvailabilityPutModel
    {
        int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
