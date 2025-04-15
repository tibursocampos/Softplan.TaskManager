using System.Diagnostics;

using TaskManager.API.Models;
using TaskManager.Core.Exceptions;

namespace TaskManager.API.Middleware;

/// <summary>
/// Middleware that handles unhandled exceptions globally and logs request and response data.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
    /// <param name="logger">The logger instance used to log information and errors.</param>
    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Executes the middleware logic: logs request/response and handles exceptions.
    /// </summary>
    /// <param name="context">The HTTP context of the current request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);

            await _next(context);

            stopwatch.Stop();
            _logger.LogInformation("Response: {StatusCode} in {Duration}ms", context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            await HandleExceptionAsync(context, ex, stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Handles exceptions by logging them and writing a structured error response.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="ex">The exception that was thrown.</param>
    /// <param name="durationMs">The duration of the request in milliseconds.</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception ex, long durationMs)
    {
        var statusCode = ex is TaskManagerException businessEx ? businessEx.StatusCode : 500;

        _logger.LogError(ex, "Exception: {Type} | Status: {Status} | Path: {Path} | Duration: {Duration}ms",
            ex.GetType().Name, statusCode, context.Request.Path, durationMs);

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new ErrorResponse(
            statusCode,
            ex is TaskManagerException ? ex.Message : "Internal server error",
            context.Response.Headers["X-Correlation-ID"].FirstOrDefault() ?? "unknown",
            context.Response.StatusCode >= 500 ? ex.Message : null));
    }
}
