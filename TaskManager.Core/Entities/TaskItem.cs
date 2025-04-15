namespace TaskManager.Core.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime DueDate { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsCompleted { get; private set; }

    public TaskItem(string title, string? description, DateTime dueDate, Guid userId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        CreationDate = DateTime.UtcNow;
        DueDate = dueDate;
        UserId = userId;
        IsCompleted = false;
    }

    public void MarkAsComplete()
    {
        IsCompleted = true;
    }

    public static TaskItem CreateForSeed(
    string title,
    string description,
    DateTime dueDate,
    Guid userId)
    {
        return new TaskItem(title, description, dueDate, userId);
    }
}