using Microsoft.Extensions.Logging;

using TaskManager.Core.Entities;
using TaskManager.Core.Exceptions;
using TaskManager.Core.Interfaces;

namespace TaskManager.Core.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TaskService> _logger;

    public TaskService(
        ITaskRepository taskRepository,
        ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task, CancellationToken cancellationToken)
    {
        if (task is null)
            throw new ValidationException("Task object cannot be null");

        _logger.LogInformation("Creating new task for user {UserId}", task.UserId);
        var createdTask = await _taskRepository.AddAsync(task, cancellationToken);
        _logger.LogInformation("Created task {TaskId} for user {UserId}", createdTask.Id, task.UserId);

        return createdTask;
    }

    public async Task<IEnumerable<TaskItem>> GetUserTasksAsync(Guid userId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Fetching tasks for user {UserId}", userId);
        var tasks = await _taskRepository.GetByUserIdAsync(userId, cancellationToken);

        if (!tasks.Any())
        {
            _logger.LogInformation("No tasks found for user {UserId} - Returning empty list", userId);
            return [];
        }

        _logger.LogDebug("Found {TaskCount} tasks for user {UserId}", tasks.Count(), userId);
        return tasks;
    }

    public async Task CompleteTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Attempting to complete task {TaskId}", taskId);
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken, trackEntity: true);

        if (task is null)
        {
            _logger.LogWarning("Task not found for completion: {TaskId}", taskId);
            throw new NotFoundException($"Task with id {taskId} not found");
        }

        task.MarkAsComplete();
        await _taskRepository.UpdateAsync(task, cancellationToken);
        _logger.LogInformation("Task {TaskId} marked as completed", taskId);
    }

    public async Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Attempting to delete task {TaskId}", taskId);
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken, trackEntity: true);

        if (task is null)
        {
            _logger.LogWarning("Task not found for deletion: {TaskId}", taskId);
            throw new NotFoundException($"Task with id {taskId} not found");
        }

        await _taskRepository.DeleteAsync(taskId, cancellationToken);
        _logger.LogInformation("Task {TaskId} deleted successfully", taskId);
    }
}