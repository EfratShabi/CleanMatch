using Clean.Core.Entities;
using Clean.Service;

namespace CleanerTest
{
    public class CleanerMatchingTests
    {
        [Fact]
        public void Weights_add_up_to_one()
        {
            Assert.Equal(1.0, CleanerMatching.RatingWeight + CleanerMatching.DistanceWeight, 10);
        }

        [Fact]
        public void A_cleaner_without_reviews_starts_at_the_prior_rating()
        {
            Assert.Equal(CleanerMatching.PriorRating, CleanerMatching.AdjustedRating(0, 0), 10);
        }

        [Fact]
        public void One_perfect_review_does_not_beat_fifty_very_good_ones()
        {
            var oneReview = CleanerMatching.AdjustedRating(5, 1);
            var manyReviews = CleanerMatching.AdjustedRating(4.8 * 50, 50);

            Assert.True(manyReviews > oneReview);
        }

        [Fact]
        public void Adjusted_rating_approaches_the_real_average_as_reviews_grow()
        {
            Assert.InRange(CleanerMatching.AdjustedRating(5 * 1000, 1000), 4.98, 5.0);
        }

        [Fact]
        public void Closer_cleaner_scores_higher_with_the_same_rating()
        {
            var near = CleanerMatching.Score(40, 10, distanceKm: 2, upToKm: 10);
            var far = CleanerMatching.Score(40, 10, distanceKm: 8, upToKm: 10);

            Assert.True(near > far);
        }

        [Fact]
        public void Better_rated_cleaner_scores_higher_at_the_same_distance()
        {
            var good = CleanerMatching.Score(48, 10, distanceKm: 5, upToKm: 10);
            var weak = CleanerMatching.Score(30, 10, distanceKm: 5, upToKm: 10);

            Assert.True(good > weak);
        }

        [Fact]
        public void Distance_is_measured_against_each_cleaners_own_range()
        {
            var sameKmShortRange = CleanerMatching.Score(40, 10, distanceKm: 5, upToKm: 6);
            var sameKmLongRange = CleanerMatching.Score(40, 10, distanceKm: 5, upToKm: 20);

            Assert.True(sameKmLongRange > sameKmShortRange);
        }

        [Fact]
        public void Distance_part_is_zero_at_the_edge_of_the_range_and_does_not_go_negative_beyond_it()
        {
            var atEdge = CleanerMatching.Score(40, 10, distanceKm: 10, upToKm: 10);
            var beyond = CleanerMatching.Score(40, 10, distanceKm: 25, upToKm: 10);

            Assert.Equal(atEdge, beyond);
        }

        [Fact]
        public void A_good_cleaner_at_half_her_range_beats_a_weak_one_right_next_door()
        {
            var goodButFarther = CleanerMatching.Score(4.8 * 50, 50, distanceKm: 5, upToKm: 10);
            var weakButClose = CleanerMatching.Score(3.0 * 20, 20, distanceKm: 1, upToKm: 10);

            Assert.True(goodButFarther > weakButClose);
        }

        [Fact]
        public void Score_stays_between_0_and_100()
        {
            var best = CleanerMatching.Score(5 * 10000, 10000, distanceKm: 0, upToKm: 10);
            var worst = CleanerMatching.Score(1 * 10000, 10000, distanceKm: 10, upToKm: 10);

            Assert.InRange(best, 99, 100);
            Assert.InRange(worst, 0, 1);
        }
    }

    public class FindBlockerTests
    {
        private static Availability Slot(int day, string start, string end) => new()
        {
            DayOfWeek = day,
            StartTime = TimeSpan.Parse(start),
            EndTime = TimeSpan.Parse(end)
        };

        // 2026-11-16 is a Monday (DayOfWeek 1).
        private static Bookings Booking(string start, string end) => new()
        {
            Date = new DateTime(2026, 11, 16),
            StartWork = TimeOnly.Parse(start),
            EndWork = TimeOnly.Parse(end)
        };

        private static readonly Availability[] Monday = { Slot(1, "08:00", "18:00") };
        private static readonly Bookings[] NoBookings = Array.Empty<Bookings>();

        [Fact]
        public void No_blocker_when_everything_fits()
        {
            Assert.Equal(BookingBlocker.None,
                BookingRules.FindBlocker(Booking("10:00", "12:00"), Monday, false, NoBookings, 3, 10));
        }

        [Fact]
        public void Blocked_date_means_not_available()
        {
            Assert.Equal(BookingBlocker.NotAvailable,
                BookingRules.FindBlocker(Booking("10:00", "12:00"), Monday, true, NoBookings, 3, 10));
        }

        [Fact]
        public void Outside_working_hours_means_not_available()
        {
            Assert.Equal(BookingBlocker.NotAvailable,
                BookingRules.FindBlocker(Booking("17:00", "19:00"), Monday, false, NoBookings, 3, 10));
        }

        [Fact]
        public void Day_without_availability_means_not_available()
        {
            Assert.Equal(BookingBlocker.NotAvailable,
                BookingRules.FindBlocker(Booking("10:00", "12:00"), new[] { Slot(2, "08:00", "18:00") }, false, NoBookings, 3, 10));
        }

        [Fact]
        public void Overlapping_booking_means_not_available()
        {
            Assert.Equal(BookingBlocker.NotAvailable,
                BookingRules.FindBlocker(Booking("10:00", "12:00"), Monday, false, new[] { Booking("11:00", "13:00") }, 3, 10));
        }

        [Fact]
        public void A_booking_after_the_required_break_is_not_a_conflict()
        {
            Assert.Equal(BookingBlocker.None,
                BookingRules.FindBlocker(Booking("13:00", "15:00"), Monday, false, new[] { Booking("09:00", "11:00") }, 3, 10));
        }

        [Fact]
        public void Missing_location_means_out_of_range()
        {
            Assert.Equal(BookingBlocker.OutOfRange,
                BookingRules.FindBlocker(Booking("10:00", "12:00"), Monday, false, NoBookings, null, 10));
        }

        [Fact]
        public void Distance_beyond_the_range_means_out_of_range()
        {
            Assert.Equal(BookingBlocker.OutOfRange,
                BookingRules.FindBlocker(Booking("10:00", "12:00"), Monday, false, NoBookings, 10.1, 10));
        }

        [Fact]
        public void Distance_exactly_equal_to_the_range_is_allowed()
        {
            Assert.Equal(BookingBlocker.None,
                BookingRules.FindBlocker(Booking("10:00", "12:00"), Monday, false, NoBookings, 10, 10));
        }

        [Fact]
        public void Not_available_wins_over_out_of_range()
        {
            Assert.Equal(BookingBlocker.NotAvailable,
                BookingRules.FindBlocker(Booking("10:00", "12:00"), Monday, true, NoBookings, 50, 10));
        }
    }
}
