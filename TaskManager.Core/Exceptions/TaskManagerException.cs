using Microsoft.AspNetCore.Http;

namespace TaskManager.Core.Exceptions;

public class TaskManagerException : Exception
{
    public int StatusCode { get; }

    public TaskManagerException(string message, int statusCode = StatusCodes.Status400BadRequest) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : TaskManagerException
{
    public NotFoundException(string message) : base(message, StatusCodes.Status404NotFound) { }
}

public class ValidationException : TaskManagerException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(Dictionary<string, string[]> errors) : base("One or more validation errors occurred")
    {
        Errors = errors;
    }
}
