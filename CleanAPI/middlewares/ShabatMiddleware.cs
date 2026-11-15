using Clean.Service;
using Microsoft.AspNetCore.Mvc;

namespace CleanAPI.middlewares
{
    public class ShabatMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TimeProvider _timeProvider;

        // TimeProvider = "what time is it now?", injected from outside so the tests can give it a fake time.
        public ShabatMiddleware(RequestDelegate next, TimeProvider timeProvider)
        {
            _next = next;
            _timeProvider = timeProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // A CORS preflight (OPTIONS) is the browser asking permission before the real request.
            // It must always get an answer, otherwise the browser shows a confusing CORS error instead of our message.
            if (HttpMethods.IsOptions(context.Request.Method))
            {
                await _next(context);
                return;
            }

            // Israel time, wherever the server happens to run (a cloud server is usually on UTC).
            var now = IsraelTime.Now(_timeProvider);
            bool isShabbat = (now.DayOfWeek == DayOfWeek.Friday && now.Hour >= 14)
                             || now.DayOfWeek == DayOfWeek.Saturday;

            if (isShabbat)
            {
                // 503 = the SERVER is unavailable (400 would wrongly say the client's request was bad)
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status503ServiceUnavailable,
                    Title = "Service unavailable on Shabbat",
                    Detail = "השירות לא זמין בשבת"
                });
                return;
            }

            await _next(context);
        }
    }
}
