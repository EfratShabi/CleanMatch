using Clean.Core.Entities;
using Clean.Core.Repositories;
using Clean.Core.Services;

namespace Clean.Service
{
    public class BookingService : IBookingService
    {
        private const string NotAvailableMessage = "המנקה לא זמינה בתאריך ובשעות האלה.";

        private readonly IBookingRepository _bookingRepository;
        private readonly ICleanerRepository _cleanerRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IBlockedDateRepository _blockedDateRepository;
        private readonly ILocationService _locationService;
        private readonly TimeProvider _timeProvider;

        public BookingService(IBookingRepository bookingRepository, ICleanerRepository cleanerRepository,
            ICustomerRepository customerRepository, IAvailabilityRepository availabilityRepository,
            IBlockedDateRepository blockedDateRepository, ILocationService locationService, TimeProvider timeProvider)
        {
            _bookingRepository = bookingRepository;
            _cleanerRepository = cleanerRepository;
            _customerRepository = customerRepository;
            _availabilityRepository = availabilityRepository;
            _blockedDateRepository = blockedDateRepository;
            _locationService = locationService;
            _timeProvider = timeProvider;
        }

        public async Task<List<Bookings>> GetBookingsAsync()
        {
            return await _bookingRepository.GetBookingsAsync();
        }
        public async Task<List<Bookings>> GetBookingByIDOfCleaners(int idOfCleaners)
        {
            return await _bookingRepository.GetBookingByIDOfCleaners(idOfCleaners);
        }
        public async Task<Bookings?> GetBookingsByIdAsync(int id)
        {
            return await _bookingRepository.GetBookingsByIdAsync(id);
        }

        public async Task AddBookingAsync(Bookings newBooking)
        {
            var cleaner = await _cleanerRepository.GetCleanerByIdAsync(newBooking.CleanersId)
                ?? throw new ArgumentException("המנקה לא נמצאה.");
            var customer = await _customerRepository.GetCustomerByIdAsync(newBooking.CustomersId)
                ?? throw new ArgumentException("הלקוחה לא נמצאה.");

            BookingRules.ValidateTimes(newBooking, IsraelTime.Now(_timeProvider));

            if (await _bookingRepository.HasBookingOnDateAsync(newBooking.CustomersId, newBooking.Date))
            {
                throw new InvalidOperationException("כבר קיימת לך הזמנה לתאריך זה.");
            }

            var day = DateOnly.FromDateTime(newBooking.Date);
            var isBlocked = (await _blockedDateRepository.GetDatesInRangeAsync(cleaner.Id, day, day)).Any();
            var weeklySlots = await _availabilityRepository.GetByCleanerIdAsync(cleaner.Id);
            if (isBlocked || !AvailabilityRules.Fits(newBooking, weeklySlots))
            {
                throw new InvalidOperationException(NotAvailableMessage);
            }

            var sameDay = await _bookingRepository.GetUpcomingBookingsOfCleanerAsync(cleaner.Id, newBooking.Date.Date);
            if (sameDay.Any(other => BookingRules.Conflicts(newBooking, other)))
            {
                throw new InvalidOperationException(NotAvailableMessage);
            }

            if (!HasLocation(customer.Latitude, customer.Longitude)
                || !HasLocation(cleaner.Latitude, cleaner.Longitude)
                || _locationService.CalculateDistance(customer.Latitude, customer.Longitude,
                       cleaner.Latitude, cleaner.Longitude) > cleaner.UpToKm)
            {
                throw new InvalidOperationException("הכתובת שלך מחוץ לטווח העבודה של המנקה.");
            }

            newBooking.TotalPrice = BookingRules.CalculatePrice(newBooking, cleaner.PricePerHour);

            await _bookingRepository.AddBookingAsync(newBooking);
            await _bookingRepository.SaveAsync();
        }

        public async Task UpdateBookingAsync(Bookings updatedBooking)
        {
            await _bookingRepository.UpdateBookingAsync(updatedBooking);
            await _bookingRepository.SaveAsync();
        }

        public async Task CancelAsync(Bookings booking, CancelledByRole by)
        {
            if (booking.Status == BookingStatus.Cancelled)
            {
                throw new InvalidOperationException("ההזמנה כבר בוטלה.");
            }
            if (booking.HasEnded(IsraelTime.Now(_timeProvider)))
            {
                throw new InvalidOperationException("אי אפשר לבטל הזמנה שכבר הסתיימה.");
            }

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = _timeProvider.GetUtcNow().UtcDateTime;
            booking.CancelledBy = by;
            await _bookingRepository.UpdateBookingAsync(booking);
            await _bookingRepository.SaveAsync();
        }

        public Task<List<Bookings>> GetByCustomerAsync(int customerId)
        {
            return _bookingRepository.GetBookingsOfCustomerAsync(customerId);
        }

        public async Task<bool> CleanerCanSeeCustomerAsync(int cleanerId, int customerId)
        {
            var now = IsraelTime.Now(_timeProvider);
            var confirmed = await _bookingRepository.GetConfirmedBookingsBetweenAsync(cleanerId, customerId, now.Date);
            return confirmed.Any(b => !b.HasEnded(now));
        }

        private static bool HasLocation(double latitude, double longitude) => latitude != 0 || longitude != 0;
    }
}
