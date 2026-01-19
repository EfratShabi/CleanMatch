using Clean.Core.Entities;
using Clean.Core.Repositories;
using Clean.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Service
{
    public class BookingService:IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public List<Bookings> GetBookings() 
        { 
            return _bookingRepository.GetBookings();
        }
        public Bookings GetBookingsById(int id) 
        {
            return _bookingRepository.GetBookingsById(id);
        }
        public async Task AddBooking(Bookings newBooking)
        {
            bool exists = await _bookingRepository.HasBookingOnDateAsync(newBooking.customerId, newBooking.Date);
            if (exists)
            {
                throw new ArgumentException("כבר קיימת לך הזמנה לתאריך זה.");
            }
            _bookingRepository.AddBooking(newBooking);
            _bookingRepository.Save();

        }

        public void UpdateBooking(Bookings updatedBooking) 
        {
            _bookingRepository.UpdateBooking(updatedBooking);
            _bookingRepository.Save();
        }

        public void DeleteBooking(int id)
        {
            _bookingRepository.DeleteBooking(id);
            _bookingRepository.Save();
        }

    }
}
