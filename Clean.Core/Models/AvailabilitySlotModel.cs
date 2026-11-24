namespace Clean.Core.Models
{
    public class AvailabilitySlotModel
    {
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
