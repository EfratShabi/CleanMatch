namespace Clean.Core.DTO
{
    // What the API returns for a booking (never the Bookings entity itself).
    public class BookingDTO
    {
        public int BookingCode { get; set; }
        public double TotalPrice { get; set; }
        public DateTime Date { get; set; }
        public TimeOnly StartWork { get; set; }
        public TimeOnly EndWork { get; set; }
        public int CustomersId { get; set; }
        public int CleanersId { get; set; }
    }
}
