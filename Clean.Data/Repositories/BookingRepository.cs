using Clean.Core.Entities;
using Clean.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Data.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DataContext _context;

        public BookingRepository(DataContext context)
        {
            _context = context;
        }

        public List<Bookings> GetBookings()
        {
            return _context.Bookings.ToList();
        }
        public Bookings GetBookingsById(int id)
        {
            var b=_context.Bookings.ToList().Find(b => b.BookingCode == id);
            return b;
        }

        public void AddBooking(Bookings newBooking)
        {
            _context.Bookings.Add(newBooking);
            
        }
        public void UpdateBooking(Bookings updatedBooking)
        {
            _context.Bookings.Update(updatedBooking);
        }

        public void DeleteBooking(int id)
        {
            _context.Bookings.Remove(GetBookingsById(id));
        }
        public async Task<bool> HasBookingOnDateAsync(int customerId, DateTime date)
        {
            return await _context.Bookings.AnyAsync(b=>
            b.customerId==customerId&&b.Date.Date==date.Date
            );
        }
        public void Save()
        {
            _context.SaveChanges();
        }

    }
}

