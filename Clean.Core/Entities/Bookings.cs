namespace Clean.Core.Entities
{
    public class Bookings
    {

        public int BookingCode { get; set; }
        public double TotalPrice { get; set; }
        
        public int CodeCustomer {  get; set; }
        public int CodeCleaner { get; set; }

        public DateTime Date { get; set; }

        public TimeOnly StartWork { get; set; }

        public TimeOnly EndWork { get; set; }







    }
}
