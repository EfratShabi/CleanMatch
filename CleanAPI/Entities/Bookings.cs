namespace CleanAPI.Entities
{
    public class Bookings
    {

        public int BookingCode { get; set; }
        public string IdOfCleaner { get; set; }
        public string IdOfCustomer { get; set; }

        public double TotalPrice { get; set; }

        public TimeOnly StartWork { get; set; }

        public TimeOnly EndWork { get; set; }







    }
}
