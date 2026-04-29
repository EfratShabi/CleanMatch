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

        public async Task<List<Bookings>> GetBookingsAsync()
        {
            return await _context.Bookings.ToListAsync();
        }

        public async Task<Bookings?> GetBookingsByIdAsync(int id)
        {

            return await _context.Bookings.FirstOrDefaultAsync(b => b.BookingCode == id);
        }
        public async Task<List<Bookings>> GetBookingByIDOfCleaners(int idOfCleaners)
        {
            return await _context.Bookings.Where(b => b.CleanersId == idOfCleaners).ToListAsync();
        }

        public async Task AddBookingAsync(Bookings newBooking)
        {
            await _context.Bookings.AddAsync(newBooking);
        }

        public async Task UpdateBookingAsync(Bookings updatedBooking)
        {
            _context.Bookings.Update(updatedBooking);
            await Task.CompletedTask;
        }

        public async Task DeleteBookingAsync(int id)
        {
            var booking = await GetBookingsByIdAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }
        }

        public async Task<bool> HasBookingOnDateAsync(int customerId, DateTime date)
        {
            return await _context.Bookings.AnyAsync(b =>
                b.CustomersId == customerId && b.Date.Date == date.Date
            );
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}