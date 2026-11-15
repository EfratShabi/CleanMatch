using CleanAPI.middlewares;
using Microsoft.AspNetCore.Http;

namespace CleanerTest
{
    // Israel is UTC+2 in winter and UTC+3 in summer.
    // 2026-11-13 is a Friday (winter), 2026-07-10 is a Friday (summer).
    public class ShabatMiddlewareTests
    {
        // A clock that always shows the time we choose.
        private sealed class FixedTimeProvider : TimeProvider
        {
            private readonly DateTimeOffset _utcNow;
            public FixedTimeProvider(DateTimeOffset utcNow) => _utcNow = utcNow;
            public override DateTimeOffset GetUtcNow() => _utcNow;
        }

        private static async Task<(bool NextCalled, int Status)> RunAsync(string utcTime, string method = "GET")
        {
            var nextCalled = false;
            var middleware = new ShabatMiddleware(
                _ => { nextCalled = true; return Task.CompletedTask; },
                new FixedTimeProvider(DateTimeOffset.Parse(utcTime)));

            var context = new DefaultHttpContext();
            context.Request.Method = method;
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context);
            return (nextCalled, context.Response.StatusCode);
        }

        [Theory]
        [InlineData("2026-11-13T11:59:00Z")] // Friday 13:59 Israel - still open
        [InlineData("2026-11-15T10:00:00Z")] // Sunday
        [InlineData("2026-11-11T10:00:00Z")] // Wednesday
        [InlineData("2026-11-14T22:30:00Z")] // Saturday 22:30 UTC = Sunday 00:30 Israel - already open
        [InlineData("2026-07-10T10:59:00Z")] // summer: Friday 13:59 Israel - still open
        public async Task Allows_requests_outside_shabbat(string utcTime)
        {
            var (nextCalled, status) = await RunAsync(utcTime);

            Assert.True(nextCalled);
            Assert.Equal(StatusCodes.Status200OK, status);
        }

        [Theory]
        [InlineData("2026-11-13T12:00:00Z")] // Friday 14:00 Israel - closes
        [InlineData("2026-11-13T12:30:00Z")] // Friday 12:30 UTC = 14:30 Israel - the old DateTime.Now code let this through on a UTC server
        [InlineData("2026-11-14T10:00:00Z")] // Saturday
        [InlineData("2026-11-14T21:30:00Z")] // Saturday 23:30 Israel - closed (the day ends at midnight)
        [InlineData("2026-07-10T11:00:00Z")] // summer: Friday 14:00 Israel (UTC+3) - closes
        public async Task Blocks_requests_during_shabbat_with_503(string utcTime)
        {
            var (nextCalled, status) = await RunAsync(utcTime);

            Assert.False(nextCalled);
            Assert.Equal(StatusCodes.Status503ServiceUnavailable, status);
        }

        [Fact]
        public async Task Lets_cors_preflight_through_even_on_shabbat()
        {
            var (nextCalled, status) = await RunAsync("2026-11-14T10:00:00Z", method: "OPTIONS");

            Assert.True(nextCalled);
            Assert.Equal(StatusCodes.Status200OK, status);
        }
    }
}
