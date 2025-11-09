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
    }
}
