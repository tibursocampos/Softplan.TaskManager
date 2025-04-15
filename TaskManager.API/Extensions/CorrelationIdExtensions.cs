using Serilog.Context;

namespace TaskManager.API.Extensions;

/// <summary>
/// Provides middleware extension to add and propagate a Correlation ID through HTTP requests.
/// </summary>
public static class CorrelationIdExtensions
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

    /// <summary>
    /// Adds a middleware to the pipeline that ensures each request has a Correlation ID.
    /// The ID is extracted from the request header if present, or generated otherwise.
    /// It is then attached to the response and pushed to the logging context.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The same application builder instance for chaining.</returns>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            var correlationId = GetOrCreateCorrelationId(context);
            ApplyCorrelationId(context, correlationId);
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await next();
            }
        });
    }

    /// <summary>
    /// Retrieves the Correlation ID from the request headers if available,
    /// or generates a new short GUID-based string otherwise.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>The correlation ID as a string.</returns>
    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        return context.Request.Headers.TryGetValue(CorrelationIdHeader, out var headerValue)
            ? headerValue.ToString()
            : Guid.NewGuid().ToString("N")[^8..];
    }

    /// <summary>
    /// Applies the Correlation ID to the response headers and sets the trace identifier.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="correlationId">The correlation ID to apply.</param>
    private static void ApplyCorrelationId(HttpContext context, string correlationId)
    {
        context.TraceIdentifier = correlationId;
        context.Response.Headers[CorrelationIdHeader] = correlationId;
    }
}
