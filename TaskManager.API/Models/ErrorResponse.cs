namespace TaskManager.API.Models;

public record ErrorResponse(
    int StatusCode,
    string Message,
    string CorrelationId,
    string? Details = null,
    DateTime Timestamp = default);
