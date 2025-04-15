using TaskManager.Core.Entities;

namespace TaskManager.API.Models;

public record CreateTaskRequest(string Title, string? Description, DateTime DueDate, Guid UserId)
{
    public static implicit operator TaskItem(CreateTaskRequest request)
    {
        var task = new TaskItem(request.Title, request.Description, request.DueDate, request.UserId);

        return task;
    }
}
