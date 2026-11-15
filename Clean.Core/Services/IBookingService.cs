using Clean.Core.Entities;

namespace Clean.Core.Services
{
    public interface IBookingService
    {
        Task<List<Bookings>> GetBookingsAsync();
        Task<Bookings?> GetBookingsByIdAsync(int id);
        Task<List<Bookings>> GetBookingByIDOfCleaners(int idOfCleaners);
        Task<List<Bookings>> GetByCustomerAsync(int customerId);
        Task AddBookingAsync(Bookings newBooking);
        Task UpdateBookingAsync(Bookings updatedBooking);
        Task CancelAsync(Bookings booking, CancelledByRole by);
        Task<bool> CleanerCanSeeCustomerAsync(int cleanerId, int customerId);
        Task<HashSet<int>> GetCleanerIdsWithActiveBookingAsync(int customerId);
    }
}
