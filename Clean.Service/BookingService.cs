using Clean.Core.Entities;
using Clean.Core.Repositories;
using Clean.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clean.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<List<Bookings>> GetBookingsAsync()
        {
            return await _bookingRepository.GetBookingsAsync();
        }

        public async Task<Bookings?> GetBookingsByIdAsync(int id)
        {
            return await _bookingRepository.GetBookingsByIdAsync(id);
        }

        public async Task AddBookingAsync(Bookings newBooking)
        {
            bool exists = await _bookingRepository.HasBookingOnDateAsync(newBooking.customerId, newBooking.Date);
            if (exists)
            {
                throw new ArgumentException("כבר קיימת לך הזמנה לתאריך זה.");
            }
            await _bookingRepository.AddBookingAsync(newBooking);
            await _bookingRepository.SaveAsync();
        }

        public async Task UpdateBookingAsync(Bookings updatedBooking)
        {
            await _bookingRepository.UpdateBookingAsync(updatedBooking);
            await _bookingRepository.SaveAsync();
        }

        public async Task DeleteBookingAsync(int id)
        {
            await _bookingRepository.DeleteBookingAsync(id);
            await _bookingRepository.SaveAsync();
        }
    }
}