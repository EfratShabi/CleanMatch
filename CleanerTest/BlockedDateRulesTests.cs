using Clean.Core.Entities;
using Clean.Service;

namespace CleanerTest
{
    public class BlockedDateRulesTests
    {
        [Fact]
        public void ExpandRange_single_day_returns_that_day()
        {
            var day = new DateOnly(2026, 12, 20);

            Assert.Equal(new[] { day }, AvailabilityRules.ExpandRange(day, day));
        }

        [Fact]
        public void ExpandRange_returns_every_day_including_both_ends()
        {
            var result = AvailabilityRules.ExpandRange(new DateOnly(2026, 12, 20), new DateOnly(2026, 12, 27));

            Assert.Equal(8, result.Count);
            Assert.Equal(new DateOnly(2026, 12, 20), result.First());
            Assert.Equal(new DateOnly(2026, 12, 27), result.Last());
        }

        [Fact]
        public void ExpandRange_crosses_month_and_year_boundaries()
        {
            var result = AvailabilityRules.ExpandRange(new DateOnly(2026, 12, 30), new DateOnly(2027, 1, 2));

            Assert.Equal(new[]
            {
                new DateOnly(2026, 12, 30), new DateOnly(2026, 12, 31),
                new DateOnly(2027, 1, 1), new DateOnly(2027, 1, 2)
            }, result);
        }

        [Fact]
        public void ExpandRange_rejects_from_after_to()
        {
            Assert.Throws<ArgumentException>(() =>
                AvailabilityRules.ExpandRange(new DateOnly(2026, 12, 27), new DateOnly(2026, 12, 20)));
        }

        [Fact]
        public void ExpandRange_accepts_exactly_the_maximum_and_rejects_one_more()
        {
            var from = new DateOnly(2026, 12, 1);

            Assert.Equal(AvailabilityRules.MaxBlockRangeDays,
                AvailabilityRules.ExpandRange(from, from.AddDays(AvailabilityRules.MaxBlockRangeDays - 1)).Count);
            Assert.Throws<ArgumentException>(() =>
                AvailabilityRules.ExpandRange(from, from.AddDays(AvailabilityRules.MaxBlockRangeDays)));
        }

        [Fact]
        public void IsBlocked_matches_by_date_and_ignores_time_of_day()
        {
            var blocked = new[] { new BlockedDate { Date = new DateOnly(2026, 12, 21) } };

            Assert.True(AvailabilityRules.IsBlocked(new DateTime(2026, 12, 21, 0, 0, 0), blocked));
            Assert.True(AvailabilityRules.IsBlocked(new DateTime(2026, 12, 21, 15, 30, 0), blocked));
            Assert.False(AvailabilityRules.IsBlocked(new DateTime(2026, 12, 22), blocked));
        }

        [Fact]
        public void IsBlocked_is_false_when_nothing_is_blocked()
        {
            Assert.False(AvailabilityRules.IsBlocked(new DateTime(2026, 12, 21), Array.Empty<BlockedDate>()));
        }
    }
}
