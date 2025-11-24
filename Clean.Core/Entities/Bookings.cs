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

        public int customerId { get; set; }
        public Customers Customers { get; set; }

        public int cleanerId { get; set; }
        public Cleaners Cleaners { get; set; }




    }
}
