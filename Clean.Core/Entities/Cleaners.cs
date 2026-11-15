using System.ComponentModel.DataAnnotations;

namespace Clean.Core.Entities
{
    public class Cleaners
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int IdOfCleaner { get; set; }//תז של המנקה
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhonNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string DescriptionOnMe { get; set; }
        public bool BornIsrael { get; set; }
        public bool HebrewSpeaker { get; set; }
        public  string Mikud { get; set; }
        public int PricePerHour { get; set; }
        public int  UpToKm { get; set; }
        public List<Feedback> ArrFidback { get; set; }

        public List<Availability> ArrAvailability { get; set; }

       public double Latitude { get; set; }//קו רוחב

         public double Longitude { get; set; }//קו אורך



        //public int ScheduleId { get; set; }
        //public Schedule Schedule { get; set; }





    }
}