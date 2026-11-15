using Clean.Core.Entities;

namespace Clean.Service
{
    public static class AvailabilityRules
    {
        public static void Validate(IReadOnlyList<Availability> slots)
        {
            foreach (var slot in slots)
            {
                if (slot.DayOfWeek < 0 || slot.DayOfWeek > (int)DayOfWeek.Saturday)
                    throw new ArgumentException("יום בשבוע חייב להיות בין 0 (ראשון) ל-6 (שבת).");

                if (slot.StartTime < TimeSpan.Zero || slot.EndTime >= TimeSpan.FromDays(1) || slot.StartTime >= slot.EndTime)
                    throw new ArgumentException("שעת ההתחלה חייבת להיות לפני שעת הסיום, בתוך אותו יום.");
            }

            foreach (var day in slots.GroupBy(s => s.DayOfWeek))
            {
                var ordered = day.OrderBy(s => s.StartTime).ToList();
                for (int i = 1; i < ordered.Count; i++)
                {
                    if (ordered[i].StartTime <= ordered[i - 1].EndTime)
                        throw new ArgumentException("טווחי שעות באותו יום לא יכולים לחפוף או לגעת זה בזה. איחדי אותם לטווח אחד.");
                }
            }
        }

        public const int MaxBlockRangeDays = 90;

        public static List<DateOnly> ExpandRange(DateOnly from, DateOnly to)
        {
            if (from > to)
                throw new ArgumentException("תאריך ההתחלה חייב להיות לפני תאריך הסיום.");

            var days = to.DayNumber - from.DayNumber + 1;
            if (days > MaxBlockRangeDays)
                throw new ArgumentException($"אפשר לחסום עד {MaxBlockRangeDays} ימים בבקשה אחת.");

            return Enumerable.Range(0, days).Select(from.AddDays).ToList();
        }

        public static bool IsBlocked(DateTime date, IEnumerable<BlockedDate> blocked) =>
            blocked.Any(b => b.Date == DateOnly.FromDateTime(date));

        public static bool Fits(Bookings booking, IEnumerable<Availability> slots)
        {
            var day = (int)booking.Date.DayOfWeek;
            return slots.Any(s => s.DayOfWeek == day
                                  && s.StartTime <= booking.StartWork.ToTimeSpan()
                                  && s.EndTime >= booking.EndWork.ToTimeSpan());
        }
    }
}
