using Clean.Core.Entities;
using Clean.Core.Models;
using Clean.Core.Repositories;
using Clean.Core.Services;

namespace Clean.Service
{
    public class CleanerSearchService : ICleanerSearchService
    {
        // Stops absurd values before TimeSpan.FromHours overflows; the same-day rule decides the realistic cases.
        private const double MaxHours = 24;

        private readonly ICleanerRepository _cleanerRepository;
        private readonly IBlockedDateRepository _blockedDateRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILocationService _locationService;
        private readonly TimeProvider _timeProvider;

        public CleanerSearchService(ICleanerRepository cleanerRepository, IBlockedDateRepository blockedDateRepository,
            IBookingRepository bookingRepository, ILocationService locationService, TimeProvider timeProvider)
        {
            _cleanerRepository = cleanerRepository;
            _blockedDateRepository = blockedDateRepository;
            _bookingRepository = bookingRepository;
            _locationService = locationService;
            _timeProvider = timeProvider;
        }

        public async Task<List<CleanerMatch>> SearchAsync(Customers customer, DateOnly date, TimeOnly start, double hours, int? maxPricePerHour)
        {
            if (hours > MaxHours)
            {
                throw new ArgumentException("משך ההזמנה ארוך מדי.");
            }

            var end = start.ToTimeSpan() + TimeSpan.FromHours(hours);
            if (end >= TimeSpan.FromDays(1))
            {
                throw new ArgumentException("ההזמנה חייבת להסתיים באותו יום.");
            }

            var requested = new Bookings
            {
                Date = date.ToDateTime(TimeOnly.MinValue),
                StartWork = start,
                EndWork = TimeOnly.FromTimeSpan(end)
            };
            BookingRules.ValidateTimes(requested, IsraelTime.Now(_timeProvider));

            if (!HasLocation(customer.Latitude, customer.Longitude))
            {
                throw new InvalidOperationException("עדכני את הכתובת בפרופיל כדי לחפש לפי מרחק.");
            }

            var cleaners = await _cleanerRepository.GetAllCleanersAsync();
            var blockedIds = await _blockedDateRepository.GetCleanerIdsBlockedOnAsync(date);
            var bookingsByCleaner = (await _bookingRepository.GetActiveBookingsOnDateAsync(requested.Date))
                .ToLookup(b => b.CleanersId);

            var matches = new List<CleanerMatch>();
            foreach (var cleaner in cleaners)
            {
                if (maxPricePerHour != null && cleaner.PricePerHour > maxPricePerHour)
                {
                    continue;
                }

                double? distance = HasLocation(cleaner.Latitude, cleaner.Longitude)
                    ? _locationService.CalculateDistance(customer.Latitude, customer.Longitude, cleaner.Latitude, cleaner.Longitude)
                    : null;

                var blocker = BookingRules.FindBlocker(requested, cleaner.ArrAvailability ?? new List<Availability>(),
                    blockedIds.Contains(cleaner.Id), bookingsByCleaner[cleaner.Id], distance, cleaner.UpToKm);
                if (blocker != BookingBlocker.None)
                {
                    continue;
                }

                var reviews = cleaner.ArrFidback ?? new List<Feedback>();
                var score = CleanerMatching.Score(reviews.Sum(f => f.Rating), reviews.Count, distance!.Value, cleaner.UpToKm);
                matches.Add(new CleanerMatch(cleaner, distance.Value, score,
                    BookingRules.CalculatePrice(requested, cleaner.PricePerHour)));
            }

            return matches.OrderByDescending(m => m.Score).ThenBy(m => m.Cleaner.PricePerHour).ToList();
        }

        private static bool HasLocation(double latitude, double longitude) => latitude != 0 || longitude != 0;
    }
}
