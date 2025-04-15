using Serilog.Context;

namespace TaskManager.API.Extensions;

public static class CorrelationIdExtensions
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

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

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        return context.Request.Headers.TryGetValue(CorrelationIdHeader, out var headerValue)
            ? headerValue.ToString()
            : Guid.NewGuid().ToString("N")[^8..];
    }

    private static void ApplyCorrelationId(HttpContext context, string correlationId)
    {
        context.TraceIdentifier = correlationId;
        context.Response.Headers[CorrelationIdHeader] = correlationId;
    }
}
