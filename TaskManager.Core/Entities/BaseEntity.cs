namespace TaskManager.Core.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreationDate { get; protected set; } = DateTime.UtcNow;
}
