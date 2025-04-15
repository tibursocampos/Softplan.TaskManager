using TaskManager.Core.Entities;

namespace TaskManager.API.Models;

public record TaskItemResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime CreationDate,
    DateTime DueDate,
    bool IsCompleted)
{
    public static implicit operator TaskItemResponse(TaskItem task) => 
        new(
            task.Id,
            task.Title,
            task.Description,
            task.CreationDate,
            task.DueDate,
            task.IsCompleted
        );
}
