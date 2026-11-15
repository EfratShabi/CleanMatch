using Clean.Core.Entities;
using Clean.Service;

namespace CleanerTest
{
    public class AvailabilityRulesTests
    {
        private static Availability Slot(int day, string start, string end) => new()
        {
            DayOfWeek = day,
            StartTime = TimeSpan.Parse(start),
            EndTime = TimeSpan.Parse(end)
        };

        // 2026-11-15 is a Sunday (DayOfWeek 0).
        private static Bookings Booking(string start, string end, string date = "2026-11-15") => new()
        {
            Date = DateTime.Parse(date),
            StartWork = TimeOnly.Parse(start),
            EndWork = TimeOnly.Parse(end)
        };

        [Fact]
        public void Validate_accepts_valid_week()
        {
            AvailabilityRules.Validate(new List<Availability>
            {
                Slot(0, "08:00", "12:00"),
                Slot(0, "14:00", "18:00"),
                Slot(3, "09:00", "17:00")
            });
        }

        [Fact]
        public void Validate_accepts_empty_week()
        {
            AvailabilityRules.Validate(new List<Availability>());
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(7)]
        public void Validate_rejects_day_out_of_range(int day)
        {
            Assert.Throws<ArgumentException>(() =>
                AvailabilityRules.Validate(new List<Availability> { Slot(day, "08:00", "12:00") }));
        }

        [Theory]
        [InlineData("12:00", "08:00")]
        [InlineData("08:00", "08:00")]
        [InlineData("08:00", "1.00:00")]
        public void Validate_rejects_bad_hours(string start, string end)
        {
            Assert.Throws<ArgumentException>(() =>
                AvailabilityRules.Validate(new List<Availability> { Slot(1, start, end) }));
        }

        [Theory]
        [InlineData("08:00", "12:00", "11:00", "15:00")]
        [InlineData("08:00", "12:00", "12:00", "16:00")]
        [InlineData("08:00", "16:00", "10:00", "12:00")]
        public void Validate_rejects_overlapping_or_touching_ranges_on_same_day(string s1, string e1, string s2, string e2)
        {
            Assert.Throws<ArgumentException>(() =>
                AvailabilityRules.Validate(new List<Availability> { Slot(2, s1, e1), Slot(2, s2, e2) }));
        }

        [Fact]
        public void Validate_allows_same_hours_on_different_days()
        {
            AvailabilityRules.Validate(new List<Availability>
            {
                Slot(1, "08:00", "12:00"),
                Slot(2, "08:00", "12:00")
            });
        }

        [Fact]
        public void Fits_when_booking_is_inside_range()
        {
            Assert.True(AvailabilityRules.Fits(Booking("09:00", "11:00"), new[] { Slot(0, "08:00", "12:00") }));
        }

        [Fact]
        public void Fits_when_booking_is_exactly_the_range()
        {
            Assert.True(AvailabilityRules.Fits(Booking("08:00", "12:00"), new[] { Slot(0, "08:00", "12:00") }));
        }

        [Theory]
        [InlineData("07:00", "09:00")]
        [InlineData("11:00", "13:00")]
        public void Does_not_fit_when_booking_sticks_out_of_range(string start, string end)
        {
            Assert.False(AvailabilityRules.Fits(Booking(start, end), new[] { Slot(0, "08:00", "12:00") }));
        }

        [Fact]
        public void Does_not_fit_on_a_day_without_availability()
        {
            Assert.False(AvailabilityRules.Fits(Booking("09:00", "11:00"), new[] { Slot(1, "08:00", "12:00") }));
        }

        [Fact]
        public void Fits_when_one_of_several_ranges_contains_the_booking()
        {
            var slots = new[] { Slot(0, "08:00", "10:00"), Slot(0, "14:00", "18:00") };
            Assert.True(AvailabilityRules.Fits(Booking("15:00", "17:00"), slots));
            Assert.False(AvailabilityRules.Fits(Booking("09:00", "15:00"), slots));
        }
    }
}
