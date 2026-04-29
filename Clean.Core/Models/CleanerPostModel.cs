using Clean.Core.Entities;

namespace CleanAPI.Models
{
    public class CleanerPostModel
    {

        public int IdOfCleaner { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PhonNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string DescriptionOnMe { get; set; }
        public bool BornIsrael { get; set; }
        public bool HebrewSpeaker { get; set; }
        public string Mikud { get; set; }
        public int PricePerHour { get; set; }
        public int UpToKm { get; set; }
        //public int ScheduleId { get; set; }

    }



}
