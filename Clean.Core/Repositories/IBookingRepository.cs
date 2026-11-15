using Clean.Core.Entities;

namespace Clean.Core.Repositories
{
    public interface IBookingRepository
    {
        Task<bool> HasBookingOnDateAsync(int customerId, DateTime date);
        Task<List<Bookings>> GetBookingsAsync();
        Task<Bookings?> GetBookingsByIdAsync(int id);
        Task<List<Bookings>> GetBookingByIDOfCleaners(int idOfCleaners);
        Task<List<Bookings>> GetBookingsOfCustomerAsync(int customerId);
        Task<List<Bookings>> GetUpcomingBookingsOfCleanerAsync(int cleanerId, DateTime fromDate);
        Task<List<Bookings>> GetConfirmedBookingsBetweenAsync(int cleanerId, int customerId, DateTime fromDate);

        Task AddBookingAsync(Bookings newBooking);
        Task UpdateBookingAsync(Bookings updatedBooking);

        Task SaveAsync();
    }
}
