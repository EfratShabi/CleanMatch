using Clean.Core.Entities;
using Clean.Service;

namespace CleanerTest
{
    public class BookingRulesTests
    {
        private static readonly DateTime Now = new(2026, 11, 15, 10, 0, 0);

        private static Bookings Booking(string start, string end, string date = "2026-11-16") => new()
        {
            Date = DateTime.Parse(date),
            StartWork = TimeOnly.Parse(start),
            EndWork = TimeOnly.Parse(end)
        };

        [Fact]
        public void ValidateTimes_accepts_a_normal_future_booking()
        {
            BookingRules.ValidateTimes(Booking("09:00", "11:00"), Now);
        }

        [Fact]
        public void ValidateTimes_accepts_exactly_45_minutes()
        {
            BookingRules.ValidateTimes(Booking("09:00", "09:45"), Now);
        }

        [Fact]
        public void ValidateTimes_rejects_44_minutes()
        {
            Assert.Throws<ArgumentException>(() => BookingRules.ValidateTimes(Booking("09:00", "09:44"), Now));
        }

        [Theory]
        [InlineData("11:00", "09:00")]
        [InlineData("09:00", "09:00")]
        public void ValidateTimes_rejects_end_not_after_start(string start, string end)
        {
            Assert.Throws<ArgumentException>(() => BookingRules.ValidateTimes(Booking(start, end), Now));
        }

        [Fact]
        public void ValidateTimes_rejects_a_booking_that_already_started()
        {
            Assert.Throws<ArgumentException>(() =>
                BookingRules.ValidateTimes(Booking("09:00", "11:00", "2026-11-15"), Now));
        }

        [Fact]
        public void ValidateTimes_accepts_later_today()
        {
            BookingRules.ValidateTimes(Booking("14:00", "16:00", "2026-11-15"), Now);
        }

        [Fact]
        public void Conflicts_when_ranges_overlap()
        {
            Assert.True(BookingRules.Conflicts(Booking("10:00", "12:00"), Booking("11:00", "13:00")));
        }

        [Fact]
        public void Conflicts_when_the_second_starts_right_after_the_first_ends()
        {
            Assert.True(BookingRules.Conflicts(Booking("10:00", "12:00"), Booking("12:00", "14:00")));
        }

        [Fact]
        public void Conflicts_when_the_break_is_shorter_than_30_minutes()
        {
            Assert.True(BookingRules.Conflicts(Booking("10:00", "12:00"), Booking("12:29", "14:00")));
        }

        [Fact]
        public void No_conflict_with_exactly_30_minutes_break()
        {
            Assert.False(BookingRules.Conflicts(Booking("10:00", "12:00"), Booking("12:30", "14:00")));
            Assert.False(BookingRules.Conflicts(Booking("12:30", "14:00"), Booking("10:00", "12:00")));
        }

        [Fact]
        public void No_conflict_on_different_days()
        {
            Assert.False(BookingRules.Conflicts(Booking("10:00", "12:00", "2026-11-16"), Booking("10:00", "12:00", "2026-11-17")));
        }

        [Fact]
        public void Conflicts_near_midnight_without_wrapping_around()
        {
            Assert.True(BookingRules.Conflicts(Booking("22:30", "23:50"), Booking("23:55", "23:59")));
        }

        [Theory]
        [InlineData("2026-11-15T10:59:00", false)]
        [InlineData("2026-11-15T11:00:00", true)]
        [InlineData("2026-11-15T11:01:00", true)]
        [InlineData("2026-11-14T23:00:00", false)]
        [InlineData("2026-11-16T00:00:00", true)]
        public void HasEnded_compares_the_end_of_the_booking_with_now(string now, bool expected)
        {
            var booking = Booking("09:00", "11:00", "2026-11-15");

            Assert.Equal(expected, booking.HasEnded(DateTime.Parse(now)));
        }

        [Fact]
        public void A_new_booking_is_confirmed_and_not_cancelled()
        {
            var booking = new Bookings();

            Assert.Equal(BookingStatus.Confirmed, booking.Status);
            Assert.Null(booking.CancelledAt);
            Assert.Null(booking.CancelledBy);
        }

        [Fact]
        public void CalculatePrice_is_hours_times_hourly_price()
        {
            Assert.Equal(100, BookingRules.CalculatePrice(Booking("09:00", "11:00"), 50));
        }

        [Fact]
        public void CalculatePrice_handles_partial_hours()
        {
            Assert.Equal(45, BookingRules.CalculatePrice(Booking("09:00", "09:45"), 60));
        }
    }
}
