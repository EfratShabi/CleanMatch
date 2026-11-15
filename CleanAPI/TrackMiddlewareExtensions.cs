using CleanAPI.middlewares;

namespace CleanAPI
{
    public static class TrackMiddlewareExtensions
    {
            public static IApplicationBuilder UseShabbat(this IApplicationBuilder builder)
            {
                return builder.UseMiddleware<ShabatMiddleware>();
            }

            public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
            {
                return builder.UseMiddleware<ErrorHandlingMiddleware>();
            }
      
    }
}
