using System.ComponentModel.DataAnnotations;

namespace Clean.Core.Entities
{
    public class Bookings
    {
        [Key]
        public int BookingCode { get; set; }
        public double TotalPrice { get; set; }

        public DateTime Date { get; set; }

        public TimeOnly StartWork { get; set; }

        public TimeOnly EndWork { get; set; }

        public int CustomersId { get; set; }
        public Customers Customers { get; set; }

        public int CleanersId { get; set; }
        public Cleaners Cleaners { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
        public DateTime? CancelledAt { get; set; }
        public CancelledByRole? CancelledBy { get; set; }

        public bool HasEnded(DateTime israelNow) => Date.Date + EndWork.ToTimeSpan() <= israelNow;



    }
}
