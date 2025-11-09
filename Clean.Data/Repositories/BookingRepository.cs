using Clean.Core.Entities;
using Clean.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Data.Repositories
{
    public class BookingRepository:IBookingRepository
    {
        private readonly DataContext _context;

        public BookingRepository(DataContext context)
        {
            _context = context;
        }

        public List<Bookings> GetBookings()
        { 
            return _context.Bookings;
        }
        public Bookings GetBookingsById(int id) 
        {
            var b= _context.Bookings.Find(b=>b.BookingCode==id);
            return b;
        }
    }
}
