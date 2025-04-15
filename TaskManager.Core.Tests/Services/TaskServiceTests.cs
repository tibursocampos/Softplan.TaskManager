namespace TaskManager.Core.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly Mock<ILogger<TaskService>> _loggerMock;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _loggerMock = new Mock<ILogger<TaskService>>();
        _service = new TaskService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact(DisplayName = "Create Task with Valid Data")]
    [Trait("Method", "CreateTaskAsync")]
    public async Task CreateTaskAsync_WithValidTask_ShouldReturnCreatedTask()
    {
        // Arrange
        var task = TaskFixtures.CreateValidTask();
        _repositoryMock.Setup(x => x.AddAsync(task)).ReturnsAsync(task);

        // Act
        var result = await _service.CreateTaskAsync(task);

        // Assert
        result.Should().BeEquivalentTo(task);
        _repositoryMock.Verify(x => x.AddAsync(task), Times.Once);
        VerifyLogger(LogLevel.Information, $"Creating new task for user {task.UserId}");
        VerifyLogger(LogLevel.Information, $"Created task {task.Id} for user {task.UserId}");
    }

    [Fact(DisplayName = "Create Task with Null Data")]
    [Trait("Method", "CreateTaskAsync")]
    public async Task CreateTaskAsync_WithNullTask_ShouldThrowValidationException()
    {
        // Arrange
        const string expectedError = "Task object cannot be null";

        // Act & Assert
        await _service.Invoking(s => s.CreateTaskAsync(null!))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage(expectedError);

        _loggerMock.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Creating new task")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()!),
            Times.Never);
    }

    [Fact(DisplayName = "Get User Tasks When Tasks Exist")]
    [Trait("Method", "GetUserTasksAsync")]
    public async Task GetUserTasksAsync_WithExistingTasks_ShouldReturnTasks()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tasks = TaskFixtures.CreateTaskList(3);
        _repositoryMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(tasks);

        // Act
        var result = await _service.GetUserTasksAsync(userId);

        // Assert
        result.Should().BeEquivalentTo(tasks);
        VerifyLogger(LogLevel.Debug, $"Fetching tasks for user {userId}");
        VerifyLogger(LogLevel.Debug, $"Found {tasks.Count} tasks for user {userId}");
    }

    [Fact(DisplayName = "Get User Tasks When No Tasks Exist")]
    [Trait("Method", "GetUserTasksAsync")]
    public async Task GetUserTasksAsync_WhenNoTasksExist_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(new List<TaskItem>());

        // Act
        var result = await _service.GetUserTasksAsync(userId);

        // Assert
        result.Should().BeEmpty();
        VerifyLogger(LogLevel.Information, $"No tasks found for user {userId} - Returning empty list");
    }

    [Fact(DisplayName = "Complete Task Successfully")]
    [Trait("Method", "CompleteTaskAsync")]
    public async Task CompleteTaskAsync_WithExistingTask_ShouldMarkAsCompleted()
    {
        // Arrange
        var task = TaskFixtures.CreateValidTask();
        _repositoryMock.Setup(x => x.GetByIdAsync(task.Id, true)).ReturnsAsync(task);

        // Act
        await _service.CompleteTaskAsync(task.Id);

        // Assert
        task.IsCompleted.Should().BeTrue();
        _repositoryMock.Verify(x => x.UpdateAsync(task), Times.Once);
        VerifyLogger(LogLevel.Information, $"Task {task.Id} marked as completed");
    }

    [Fact(DisplayName = "Complete Task When Task Does Not Exist")]
    [Trait("Method", "CompleteTaskAsync")]
    public async Task CompleteTaskAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetByIdAsync(taskId, true)).ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await _service.Invoking(s => s.CompleteTaskAsync(taskId))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Task with id {taskId} not found");

        VerifyLogger(LogLevel.Warning, $"Task not found for completion: {taskId}");
    }

    [Fact(DisplayName = "Delete Task Successfully")]
    [Trait("Method", "DeleteTaskAsync")]
    public async Task DeleteTaskAsync_WithExistingTask_ShouldDeleteSuccessfully()
    {
        // Arrange
        var task = TaskFixtures.CreateValidTask();
        _repositoryMock.Setup(x => x.GetByIdAsync(task.Id, true)).ReturnsAsync(task);

        // Act
        await _service.DeleteTaskAsync(task.Id);

        // Assert
        _repositoryMock.Verify(x => x.DeleteAsync(task.Id), Times.Once);
        VerifyLogger(LogLevel.Information, $"Task {task.Id} deleted successfully");
    }

    [Fact(DisplayName = "Delete Task When Task Does Not Exist")]
    [Trait("Method", "DeleteTaskAsync")]
    public async Task DeleteTaskAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetByIdAsync(taskId, true)).ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await _service.Invoking(s => s.DeleteTaskAsync(taskId))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Task with id {taskId} not found");

        VerifyLogger(LogLevel.Warning, $"Task not found for deletion: {taskId}");
    }

    private void VerifyLogger(LogLevel level, string message)
    {
        _loggerMock.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()!),
            Times.AtLeastOnce);
    }
}
