namespace TaskManager.API.Tests.Controllers;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _taskServiceMock;
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _taskServiceMock = new Mock<ITaskService>();
        _controller = new TasksController(_taskServiceMock.Object);
    }

    [Fact(DisplayName = "CreateTask com request válido retorna CreatedAtAction com dados esperados")]
    [Trait("Controller", "Task")]
    public async Task CreateTask_WithValidRequest_ReturnsCreatedResult()
    {
        // Arrange
        var request = TaskFixtures.GetValidCreateTaskRequest();
        var expectedTask = TaskFixtures.GetTaskItemWithSpecifics(
            title: request.Title,
            description: request.Description,
            dueDate: request.DueDate,
            userId: request.UserId
        );

        _taskServiceMock.Setup(x => x.CreateTaskAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _controller.CreateTask(request);

        // Assert
        var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtActionResult.ActionName.Should().Be(nameof(_controller.GetUserTasks));
        createdAtActionResult.Value.Should().BeEquivalentTo((TaskItemResponse)expectedTask);
        createdAtActionResult.RouteValues["userId"].Should().Be(expectedTask.UserId);

        _taskServiceMock.Verify(x => x.CreateTaskAsync(It.Is<TaskItem>(t =>
            t.Title == request.Title &&
            t.Description == request.Description &&
            t.DueDate == request.DueDate &&
            t.UserId == request.UserId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = "GetUserTasks retorna lista de tarefas correta para cada cenário")]
    [Trait("Controller", "Task")]
    [MemberData(nameof(GetUserTasksTestData))]
    public async Task GetUserTasks_ReturnsCorrectTasks(Guid userId, int taskCount)
    {
        // Arrange
        var tasks = TaskFixtures.GetTaskItemsForUser(userId, taskCount);

        _taskServiceMock.Setup(x => x.GetUserTasksAsync(userId, CancellationToken.None))
            .ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetUserTasks(userId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTasks = okResult.Value.Should().BeAssignableTo<IEnumerable<TaskItemResponse>>().Subject;
        returnedTasks.Should().HaveCount(taskCount);
        returnedTasks.Should().OnlyContain(t => tasks.Any(original => original.Id == t.Id));

        _taskServiceMock.Verify(x => x.GetUserTasksAsync(userId, CancellationToken.None), Times.Once);
    }

    public static IEnumerable<object[]> GetUserTasksTestData()
    {
        yield return new object[] { Guid.NewGuid(), 0 }; // No tasks
        yield return new object[] { Guid.NewGuid(), 1 }; // Single task
        yield return new object[] { Guid.NewGuid(), 5 }; // Multiple tasks
    }

    [Fact(DisplayName = "CompleteTask com ID válido retorna NoContent")]
    [Trait("Controller", "Task")]
    public async Task CompleteTask_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _taskServiceMock.Setup(x => x.CompleteTaskAsync(taskId, CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CompleteTask(taskId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _taskServiceMock.Verify(x => x.CompleteTaskAsync(taskId, CancellationToken.None), Times.Once);
    }

    [Fact(DisplayName = "DeleteTask com ID válido retorna NoContent")]
    [Trait("Controller", "Task")]
    public async Task DeleteTask_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _taskServiceMock.Setup(x => x.DeleteTaskAsync(taskId, CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteTask(taskId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _taskServiceMock.Verify(x => x.DeleteTaskAsync(taskId, CancellationToken.None), Times.Once);
    }
}
