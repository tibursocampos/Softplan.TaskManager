using TaskManager.Core.Entities;
using TaskManager.Core.Exceptions;

namespace TaskManager.Core.Interfaces;

/// <summary>
/// Defines operations for managing tasks in the system
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Creates a new task for the specified user
    /// </summary>
    /// <param name="task">The task to be created. Must include UserId.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>The created task with generated Id</returns>
    /// <exception cref="TaskManagerException">Thrown when validation fails</exception>
    /// <remarks>
    /// Logs information about task creation including UserId and generated TaskId
    /// </remarks>
    Task<TaskItem> CreateTaskAsync(TaskItem task, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all tasks for a specific user
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>List of user's tasks or empty collection if none found</returns>
    /// <remarks>
    /// Returns empty collection (not null) when no tasks are found.
    /// Logs debug information about the operation.
    /// </remarks>
    Task<IEnumerable<TaskItem>> GetUserTasksAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Marks a task as completed
    /// </summary>
    /// <param name="taskId">The unique identifier of the task</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <exception cref="NotFoundException">Thrown when task with specified id doesn't exist</exception>
    /// <remarks>
    /// Logs warning if task not found, logs information on successful completion
    /// </remarks>
    Task CompleteTaskAsync(Guid taskId, CancellationToken cancellationToken);

    /// <summary>
    /// Permanently deletes a task
    /// </summary>
    /// <param name="taskId">The unique identifier of the task</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <exception cref="NotFoundException">Thrown when task with specified id doesn't exist</exception>
    /// <remarks>
    /// Logs warning if task not found, logs information on successful deletion
    /// </remarks>
    Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken);
}