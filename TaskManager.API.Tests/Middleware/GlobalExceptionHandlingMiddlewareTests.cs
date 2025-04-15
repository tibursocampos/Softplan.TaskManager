namespace TaskManager.API.Tests.Middleware;

public class GlobalExceptionHandlingMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<ILogger<GlobalExceptionHandlingMiddleware>> _loggerMock;
    private readonly GlobalExceptionHandlingMiddleware _middleware;

    public GlobalExceptionHandlingMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerMock = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
        _middleware = new GlobalExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
    }

    [Fact(DisplayName = "Should log request and response when no exception occurs")]
    [Trait("Category", "Middleware")]
    public async Task InvokeAsync_WhenNoException_LogsRequestAndResponse()
    {
        // Arrange
        var context = MiddlewareFixtures.CreateHttpContext();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Request: GET /api/tasks")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Response: 200")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact(DisplayName = "Should return custom error response when business exception occurs")]
    [Trait("Category", "Middleware")]
    public async Task InvokeAsync_WhenBusinessException_ReturnsCustomErrorResponse()
    {
        // Arrange
        var context = MiddlewareFixtures.CreateHttpContext();
        context.Response.Headers["X-Correlation-ID"] = "test123";

        var exception = MiddlewareFixtures.GetBusinessException("Task not found", 404);
        _nextMock.Setup(x => x.Invoke(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        Assert.Contains("\"statusCode\":404", responseBody);
        Assert.Contains("\"message\":\"Task not found\"", responseBody);
        Assert.Contains("\"correlationId\":\"test123\"", responseBody);
        Assert.Equal(404, context.Response.StatusCode);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Exception: TaskManagerException")),
                exception,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact(DisplayName = "Should return 500 error response when unhandled exception occurs")]
    [Trait("Category", "Middleware")]
    public async Task InvokeAsync_WhenUnhandledException_Returns500ErrorResponse()
    {
        // Arrange
        var context = MiddlewareFixtures.CreateHttpContext();
        context.Response.Headers["X-Correlation-ID"] = "test123";
                
        var exception = MiddlewareFixtures.GetUnhandledException("Unexpected error");
        _nextMock.Setup(x => x.Invoke(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        Assert.Contains("\"statusCode\":500", responseBody);
        Assert.Contains("\"message\":\"Internal server error\"", responseBody);
        Assert.Contains("\"correlationId\":\"test123\"", responseBody);
        Assert.Contains("\"details\":\"Unexpected error\"", responseBody);
        Assert.Equal(500, context.Response.StatusCode);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Exception: Exception")),
                exception,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact(DisplayName = "Should include duration in logs when exception occurs")]
    [Trait("Category", "Middleware")]
    public async Task InvokeAsync_WhenException_IncludesDurationInLogs()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new Exception("Test error");
        _nextMock.Setup(x => x.Invoke(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Duration:")),
                exception,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }
}
