using Clean.Core.Entities;

namespace Clean.Core.Models
{
    public class CleanerPutModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhonNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string DescriptionOnMe { get; set; }
        public string Mikud { get; set; }
        public int PricePerHour { get; set; }
        public int UpToKm { get; set; }
        public int ScheduleId { get; set; }
    }
}
