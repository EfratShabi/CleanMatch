using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Core.Repositories
{
    public interface IBookingRepository
    {
        public List<Bookings> GetBookings();
        public Bookings GetBookingsById(int id);
    }
}
