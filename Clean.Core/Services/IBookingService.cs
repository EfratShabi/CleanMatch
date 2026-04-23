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
        Task<List<Bookings>> GetBookingsAsync();
        Task<Bookings?> GetBookingsByIdAsync(int id);
        Task<List<Bookings>> GetBookingByIDOfCleaners(int idOfCleaners);
        Task AddBookingAsync(Bookings newBooking);
        Task UpdateBookingAsync(Bookings updatedBooking);
        Task DeleteBookingAsync(int id);

    }
}