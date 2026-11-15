namespace Clean.Service
{
    public static class IsraelTime
    {
        public static readonly TimeZoneInfo Zone = Find();

        public static DateTime Now(TimeProvider timeProvider) =>
            TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), Zone).DateTime;

        private static TimeZoneInfo Find()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("Asia/Jerusalem"); }
            catch (TimeZoneNotFoundException) { return TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time"); }
        }
    }
}
