using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks; // חובה בשביל Task

namespace Clean.Core.Repositories
{
    public interface IBookingRepository
    {
        Task<bool> HasBookingOnDateAsync(int customerId, DateTime date);
        Task<List<Bookings>> GetBookingsAsync();
        Task<Bookings?> GetBookingsByIdAsync(int id);
        Task<List<Bookings>> GetBookingByIDOfCleaners(int idOfCleaners);
        Task<List<Bookings>> GetUpcomingBookingsOfCleanerAsync(int cleanerId, DateTime fromDate);

        Task AddBookingAsync(Bookings newBooking);
        Task UpdateBookingAsync(Bookings updatedBooking);
        Task DeleteBookingAsync(int id);

        // שמירה אסינכרונית
        Task SaveAsync();
    }
}