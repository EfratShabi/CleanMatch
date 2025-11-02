namespace CleanAPI.Entities
{
    public class Cleaners
    {
       
        public int Id { get; set; }
        public string IdOfCleaner { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhonNumber { get; set; }
        public int  CodeCity { get; set; }
        public string DescriptionOnMe { get; set; }
        public bool BornIsrael { get; set; }
        public bool availability { get; set; }
        public bool HebrewSpeaker { get; set; }
         public  string Mikud { get; set; }
       
        public int PricePerHour { get; set; }

        public int  UpToKm { get; set; }

        public List<string> ArrFidback { get; set; }

        public List<Bookings>  ArrBookings { get; set; }


    }
}