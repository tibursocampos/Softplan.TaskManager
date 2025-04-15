using Microsoft.AspNetCore.Http;

using System.Text;

using TaskManager.Core.Exceptions;

namespace TaskManager.TestUtils.Fixtures;

public static class MiddlewareFixtures
{
    public static HttpContext CreateHttpContext(
        string path = "/api/tasks",
        string method = "GET",
        string correlationId = null,
        int statusCode = 200)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        context.Request.Method = method;
        context.Response.StatusCode = statusCode;
        context.Response.Body = new MemoryStream();

        if (!string.IsNullOrEmpty(correlationId))
            context.Response.Headers["X-Correlation-ID"] = correlationId;

        return context;
    }

    public static string ReadResponseBody(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    public static Exception GetBusinessException(string message = "Business error", int statusCode = 400)
    {
        return new TaskManagerException(message, statusCode);
    }

    public static Exception GetUnhandledException(string message = "Unhandled error")
    {
        return new Exception(message);
    }
}
