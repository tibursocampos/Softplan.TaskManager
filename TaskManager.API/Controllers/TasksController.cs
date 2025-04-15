using Microsoft.AspNetCore.Mvc;

using TaskManager.API.Models;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
    }

    /// <summary>
    /// Creates a new task
    /// </summary>
    /// <response code="201">Returns the newly created task</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(TaskItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        TaskItem task = request;
        var createdTask = await _taskService.CreateTaskAsync(task);

        return CreatedAtAction(nameof(GetUserTasks), new { userId = createdTask.UserId }, (TaskItemResponse)createdTask);
    }

    /// <summary>
    /// Gets all tasks for a specific user
    /// </summary>
    /// <response code="200">Returns the user's tasks or a empty list</response>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(List<TaskItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserTasks(Guid userId)
    {
        var tasks = await _taskService.GetUserTasksAsync(userId);
        return Ok(tasks.Select(task => (TaskItemResponse)task).ToList());
    }

    /// <summary>
    /// Marks a task as completed
    /// </summary>
    /// <response code="204">If the task was successfully completed</response>
    /// <response code="404">If the task is not found</response>
    [HttpPut("{id}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteTask(Guid id)
    {
        await _taskService.CompleteTaskAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Deletes a specific task
    /// </summary>
    /// <response code="204">If the task was successfully deleted</response>
    /// <response code="404">If the task is not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        await _taskService.DeleteTaskAsync(id);
        return NoContent();
    }
}