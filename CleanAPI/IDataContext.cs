
using Clean.Core.Entities;

namespace CleanAPI
{
    public interface IDataContext
    {
        public List<Cleaners> Cleaners { get; set; }
        public List<Customers> Customers { get; set; }
        public List<Bookings> Bookings { get; set; }
        public List<Feedback> Feedback { get; set; }
        public List<User> Users { get; set; }
        public List<Availability> Availabilities { get; set; }
    }
}
