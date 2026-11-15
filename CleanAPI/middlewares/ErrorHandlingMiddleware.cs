using Microsoft.AspNetCore.Mvc;

namespace CleanAPI.middlewares
{
    // Catches any exception thrown further down the pipeline (Controller, Service, database),
    // logs the full details on the server, and returns a short, safe JSON answer to the client.
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);              // pass the request on to the next stations / the Controller
            }
            catch (Exception ex)                   // something below threw an exception
            {
                _logger.LogError(ex, "Unhandled exception. TraceId={TraceId} {Method} {Path}",
                    context.TraceIdentifier, context.Request.Method, context.Request.Path);

                if (context.Response.HasStarted)   // the answer was already partly sent, too late to change it
                {
                    throw;
                }

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred."
                };
                problem.Extensions["traceId"] = context.TraceIdentifier;
                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}
