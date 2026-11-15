using Clean.Core.Entities;
using Clean.Core.Repositories;
using Microsoft.EntityFrameworkCore;

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
            return await _context.Bookings
                .Where(b => b.CleanersId == idOfCleaners)
                .OrderByDescending(b => b.Date).ThenByDescending(b => b.StartWork)
                .ToListAsync();
        }

        public async Task<List<Bookings>> GetBookingsOfCustomerAsync(int customerId)
        {
            return await _context.Bookings
                .Where(b => b.CustomersId == customerId)
                .OrderByDescending(b => b.Date).ThenByDescending(b => b.StartWork)
                .ToListAsync();
        }

        public async Task<List<Bookings>> GetUpcomingBookingsOfCleanerAsync(int cleanerId, DateTime fromDate)
        {
            return await _context.Bookings
                .Where(b => b.CleanersId == cleanerId && b.Date >= fromDate && b.Status != BookingStatus.Cancelled)
                .ToListAsync();
        }

        public async Task<List<Bookings>> GetConfirmedBookingsBetweenAsync(int cleanerId, int customerId, DateTime fromDate)
        {
            return await _context.Bookings
                .Where(b => b.CleanersId == cleanerId && b.CustomersId == customerId
                            && b.Date >= fromDate && b.Status == BookingStatus.Confirmed)
                .ToListAsync();
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

        public async Task<bool> HasBookingOnDateAsync(int customerId, DateTime date)
        {
            return await _context.Bookings.AnyAsync(b =>
                b.CustomersId == customerId && b.Date.Date == date.Date && b.Status != BookingStatus.Cancelled
            );
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
