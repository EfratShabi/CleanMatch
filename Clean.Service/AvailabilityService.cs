using Clean.Core.Entities;
using Clean.Core.Repositories;
using Clean.Core.Services;

namespace Clean.Service
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly TimeProvider _timeProvider;

        public AvailabilityService(IAvailabilityRepository availabilityRepository,
            IBookingRepository bookingRepository, TimeProvider timeProvider)
        {
            _availabilityRepository = availabilityRepository;
            _bookingRepository = bookingRepository;
            _timeProvider = timeProvider;
        }

        public Task<List<Availability>> GetByCleanerAsync(int cleanerId)
        {
            return _availabilityRepository.GetByCleanerIdAsync(cleanerId);
        }

        public async Task ReplaceWeekAsync(int cleanerId, List<Availability> slots)
        {
            AvailabilityRules.Validate(slots);

            var from = _timeProvider.GetUtcNow().UtcDateTime.Date;
            var upcoming = await _bookingRepository.GetUpcomingBookingsOfCleanerAsync(cleanerId, from);
            var outside = upcoming.FirstOrDefault(b => !AvailabilityRules.Fits(b, slots));
            if (outside != null)
            {
                throw new InvalidOperationException(
                    $"יש לך הזמנה ב-{outside.Date:dd/MM/yyyy} בין {outside.StartWork:HH:mm} ל-{outside.EndWork:HH:mm} שלא נכנסת בשעות החדשות. בטלי אותה קודם או הרחיבי את השעות.");
            }

            foreach (var slot in slots)
            {
                slot.CleanerId = cleanerId;
            }
            await _availabilityRepository.ReplaceForCleanerAsync(cleanerId, slots);
            await _availabilityRepository.SaveAsync();
        }
    }
}
