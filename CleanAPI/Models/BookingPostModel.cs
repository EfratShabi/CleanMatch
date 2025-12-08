using Clean.Core.Entities;

namespace CleanAPI.Models
{
    public class BookingPostModel
    {
        public double TotalPrice { get; set; }

        public DateTime Date { get; set; }

        public TimeOnly StartWork { get; set; }

        public TimeOnly EndWork { get; set; }

        public int customerId { get; set; }

        public int cleanerId { get; set; }
    }
}
