using Clean.Core.Entities;

namespace Clean.Core.Models
{
    public class BookingPostModel
    {

        public DateTime Date { get; set; }

        public TimeOnly StartWork { get; set; }

        public TimeOnly EndWork { get; set; }

        // CustomersId is NOT here on purpose: the customer is taken from the login token, not from the request body.
        public int CleanersId { get; set; }

    }
}
