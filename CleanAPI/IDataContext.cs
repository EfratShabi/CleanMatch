using CleanAPI.Entities;

namespace CleanAPI
{
    public interface IDataContext
    {
        public List<Cleaners> Cleaners { get; set; }
        public List<Customers> Customers { get; set; }
        public List<Bookings> Bookings { get; set; }
        public List<Schedule> schedules { get; set; }
    }
}
