namespace TaskManager.Infra.Tests.Repositories;

public class TaskRepositoryTests
{
    private readonly TaskManagerContext _context;
    private readonly TaskRepository _repository;

    public TaskRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TaskManagerContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new TaskManagerContext(options);
        _context.Database.EnsureCreated();
        _repository = new TaskRepository(_context);
    }

    [Fact(DisplayName = "Add Task - Should Add Entity to Database")]
    [Trait("Method", "AddAsync")]
    public async Task AddAsync_ShouldAddEntityToDatabase()
    {
        // Arrange
        var task = TaskFixtures.CreateValidTask();

        // Act
        var result = await _repository.AddAsync(task, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(task.Id);
        _context.Tasks.Should().Contain(task);
    }

    [Fact(DisplayName = "Get Tasks By User ID - Should Return Tasks For User With Correct Properties")]
    [Trait("Method", "GetByUserIdAsync")]
    public async Task GetByUserIdAsync_ShouldReturnTasksForUserWithCorrectProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var task1 = TaskFixtures.CreateValidTask(userId);
        var task2 = TaskFixtures.CreateValidTask(userId);
        var task3 = TaskFixtures.CreateValidTask(userId);

        _context.Tasks.AddRange(task1, task2, task3);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByUserIdAsync(userId, CancellationToken.None)).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);

        result.Should().Contain(task =>
            task.Title == task1.Title &&
            task.Description == task1.Description &&
            task.DueDate == task1.DueDate);

        result.Should().Contain(task =>
            task.Title == task2.Title &&
            task.Description == task2.Description &&
            task.DueDate == task2.DueDate);

        result.Should().Contain(task =>
            task.Title == task3.Title &&
            task.Description == task3.Description &&
            task.DueDate == task3.DueDate);
    }

    [Fact(DisplayName = "Get Tasks By User ID - Should Return Empty List When No Tasks Exist")]
    [Trait("Method", "GetByUserIdAsync")]
    public async Task GetByUserIdAsync_ShouldReturnEmptyListWhenNoTasksExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByUserIdAsync(userId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact(DisplayName = "Get Task By ID - Should Return Task By ID")]
    [Trait("Method", "GetByIdAsync")]
    public async Task GetByIdAsync_ShouldReturnTaskById()
    {
        // Arrange
        var task = TaskFixtures.CreateValidTask();

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(task.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(task.Id);
    }

    [Fact(DisplayName = "Get Task By ID - Should Return Null When Task Does Not Exist")]
    [Trait("Method", "GetByIdAsync")]
    public async Task GetByIdAsync_ShouldReturnNullWhenTaskDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact(DisplayName = "Update Task - Should Update Task")]
    [Trait("Method", "UpdateAsync")]
    public async Task UpdateAsync_ShouldUpdateTask()
    {
        // Arrange
        var task = TaskFixtures.CreateValidTask();

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        task.MarkAsComplete();

        // Act
        await _repository.UpdateAsync(task, CancellationToken.None);
        var updatedTask = await _context.Tasks.FindAsync(task.Id);

        // Assert
        updatedTask.Should().NotBeNull();
        updatedTask.IsCompleted.Should().BeTrue();
    }

    [Fact(DisplayName = "Update Task - Should Throw Concurrency Exception When Task Does Not Exist")]
    [Trait("Method", "UpdateAsync")]
    public async Task UpdateAsync_ShouldThrowConcurrencyExceptionWhenTaskDoesNotExist()
    {
        // Arrange
        var task = TaskFixtures.CreateValidTask();

        // Act & Assert
        await _repository.Invoking(r => r.UpdateAsync(task, CancellationToken.None))
            .Should().ThrowAsync<DbUpdateConcurrencyException>();
    }

    [Fact(DisplayName = "Delete Task - Should Remove Task")]
    [Trait("Method", "DeleteAsync")]
    public async Task DeleteAsync_ShouldRemoveTask()
    {
        // Arrange
        var task = TaskFixtures.CreateValidTask();

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(task.Id, CancellationToken.None);
        var deletedTask = await _context.Tasks.FindAsync(task.Id);

        // Assert
        deletedTask.Should().BeNull();
    }

    [Fact(DisplayName = "Delete Task - Should Not Throw Exception When Task Does Not Exist")]
    [Trait("Method", "DeleteAsync")]
    public async Task DeleteAsync_ShouldNotThrowExceptionWhenTaskDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await _repository.Invoking(r => r.DeleteAsync(nonExistentId, CancellationToken.None))
            .Should().NotThrowAsync();
    }
}