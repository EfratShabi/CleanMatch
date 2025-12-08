namespace Clean.Core.Entities
{
    public class Cleaners
    {
       
        public int Id { get; set; }
        public int IdOfCleaner { get; set; }//תז של המנקה
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhonNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string DescriptionOnMe { get; set; }
        public bool BornIsrael { get; set; }
        public bool availability { get; set; }
        public bool HebrewSpeaker { get; set; }
        public  string Mikud { get; set; }
        public int PricePerHour { get; set; }
        public int  UpToKm { get; set; }
        public List<Feedback> ArrFidback { get; set; }
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }

        



    }
}