using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Core.Services
{
    public interface IBookingService
    {
        public List<Bookings> GetBookings();
        public Bookings GetBookingsById(int id);
        public Task AddBooking(Bookings newBooking);
        public void UpdateBooking(Bookings updatedBooking);
        public void DeleteBooking(int id);

    }
}
