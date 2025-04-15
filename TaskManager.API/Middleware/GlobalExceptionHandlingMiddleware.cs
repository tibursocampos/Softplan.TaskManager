using System.Diagnostics;

using TaskManager.API.Models;
using TaskManager.Core.Exceptions;

namespace TaskManager.API.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

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
