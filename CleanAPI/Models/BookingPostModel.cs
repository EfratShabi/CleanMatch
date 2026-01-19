using Clean.Core.Entities;

namespace CleanAPI.Models
{
    public class BookingPostModel
    {

        public DateTime Date { get; set; }

        public TimeOnly StartWork { get; set; }

        public TimeOnly EndWork { get; set; }

        public int customerId { get; set; }

    }
}
