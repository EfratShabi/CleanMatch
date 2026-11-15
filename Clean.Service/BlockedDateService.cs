using Clean.Core.Entities;
using Clean.Core.Repositories;
using Clean.Core.Services;

namespace Clean.Service
{
    public class BlockedDateService : IBlockedDateService
    {
        private readonly IBlockedDateRepository _repository;
        private readonly IBookingRepository _bookingRepository;
        private readonly TimeProvider _timeProvider;

        public BlockedDateService(IBlockedDateRepository repository, IBookingRepository bookingRepository, TimeProvider timeProvider)
        {
            _repository = repository;
            _bookingRepository = bookingRepository;
            _timeProvider = timeProvider;
        }

        private DateOnly Today => DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);

        public Task<List<BlockedDate>> GetUpcomingAsync(int cleanerId)
        {
            return _repository.GetFromDateAsync(cleanerId, Today);
        }

        public async Task BlockAsync(int cleanerId, DateOnly from, DateOnly to)
        {
            if (from < Today)
            {
                throw new ArgumentException("אי אפשר לחסום תאריך שעבר.");
            }
            var dates = AvailabilityRules.ExpandRange(from, to);

            var upcoming = await _bookingRepository.GetUpcomingBookingsOfCleanerAsync(cleanerId, from.ToDateTime(TimeOnly.MinValue));
            var conflict = upcoming.FirstOrDefault(b => dates.Contains(DateOnly.FromDateTime(b.Date)));
            if (conflict != null)
            {
                throw new InvalidOperationException(
                    $"יש לך הזמנה ב-{conflict.Date:dd/MM/yyyy}. בטלי אותה קודם ואז חסמי את התאריך.");
            }

            var alreadyBlocked = await _repository.GetDatesInRangeAsync(cleanerId, from, to);
            await _repository.AddRangeAsync(dates.Except(alreadyBlocked)
                .Select(d => new BlockedDate { CleanerId = cleanerId, Date = d }));
            await _repository.SaveAsync();
        }

        public async Task UnblockAsync(int cleanerId, DateOnly from, DateOnly to)
        {
            AvailabilityRules.ExpandRange(from, to);
            await _repository.RemoveRangeAsync(cleanerId, from, to);
            await _repository.SaveAsync();
        }
    }
}
