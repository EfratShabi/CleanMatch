namespace CleanAPI.middlewares
{
    public class ShabatMiddleware
    {
        private readonly RequestDelegate _next;

        public ShabatMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var today = DateTime.Now;
            if (today.DayOfWeek == DayOfWeek.Friday && today.Hour >= 14 || today.DayOfWeek == DayOfWeek.Saturday)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync("השירות לא זמין בשבת");
                return;
            }

            await _next(context);
        }
    }
}