using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces;

/// <summary>
/// Defines the contract for task data persistence operations
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Persists a new task entity in the database
    /// </summary>
    /// <param name="task">The task entity to be added</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>The persisted task with any database-generated values</returns>
    /// <remarks>
    /// The implementation should handle the complete transaction including:
    /// - Adding the entity to the context
    /// - Saving changes to the database
    /// - Returning the entity with any generated IDs or timestamps
    /// </remarks>
    Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all tasks associated with a specific user
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>
    /// A collection of tasks for the specified user.
    /// The collection will be empty if no tasks are found.
    /// </returns>
    /// <remarks>
    /// The implementation uses AsNoTracking() by default for better performance.
    /// Changes to returned entities will not be persisted unless explicitly saved.
    /// </remarks>
    Task<IEnumerable<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Finds a task by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the task</param>
    /// <param name="trackEntity">
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// When true, the returned entity will be tracked by the context.
    /// When false (default), uses AsNoTracking() for better performance.
    /// </param>
    /// <returns>
    /// The task entity if found, otherwise null.
    /// </returns>
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool trackEntity = false);

    /// <summary>
    /// Updates an existing task entity in the database
    /// </summary>
    /// <param name="task">The task entity with updated values</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <remarks>
    /// The implementation should:
    /// - Mark the entity as modified
    /// - Persist all changes to the database
    /// - Handle concurrency conflicts if applicable
    /// </remarks>
    Task UpdateAsync(TaskItem task, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a task from the database
    /// </summary>
    /// <param name="id">The unique identifier of the task to delete</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <remarks>
    /// If no task exists with the specified ID, the operation will complete silently.
    /// The implementation should:
    /// - Locate the entity
    /// - Remove it from the context
    /// - Persist the deletion
    /// </remarks>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}