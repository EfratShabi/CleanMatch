using Clean.Core.Entities;

namespace Clean.Service
{
    public static class BookingRules
    {
        public static readonly TimeSpan MinDuration = TimeSpan.FromMinutes(45);
        public static readonly TimeSpan BreakBetweenBookings = TimeSpan.FromMinutes(30);

        public static void ValidateTimes(Bookings booking, DateTime israelNow)
        {
            if (booking.EndWork <= booking.StartWork)
                throw new ArgumentException("שעת הסיום חייבת להיות אחרי שעת ההתחלה.");

            if (booking.EndWork - booking.StartWork < MinDuration)
                throw new ArgumentException("ההזמנה חייבת להיות לפחות 45 דקות.");

            if (booking.Date.Date + booking.StartWork.ToTimeSpan() <= israelNow)
                throw new ArgumentException("אי אפשר להזמין לזמן שכבר עבר.");
        }

        // TimeSpan instead of TimeOnly.Add: TimeOnly wraps past midnight (23:50 + 30 min = 00:20) and would hide a conflict.
        public static bool Conflicts(Bookings a, Bookings b)
        {
            if (a.Date.Date != b.Date.Date) return false;

            return a.StartWork.ToTimeSpan() < b.EndWork.ToTimeSpan() + BreakBetweenBookings
                && b.StartWork.ToTimeSpan() < a.EndWork.ToTimeSpan() + BreakBetweenBookings;
        }

        public static double CalculatePrice(Bookings booking, int pricePerHour) =>
            Math.Round((booking.EndWork - booking.StartWork).TotalHours * pricePerHour, 2);
    }
}
