namespace TaskManager.Integration.Tests.Controllers;

public class TasksControllerIntegrationTests : BaseIntegrationTest
{
    private const string BaseUrl = "/api/tasks";

    public TasksControllerIntegrationTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact(DisplayName = "Should create task and return Created")]
    [Trait("Category", "Integration")]
    public async Task CreateTask_WithValidRequest_ReturnsCreated()
    {
        var request = TaskFixtures.GetValidCreateTaskRequest();

        var response = await _client.PostAsync(BaseUrl, GetJsonContent(request));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await DeserializeResponse<TaskItemResponse>(response);
        createdTask.Should().NotBeNull();
        createdTask.Title.Should().Be(request.Title);
        createdTask.Description.Should().Be(request.Description);
        createdTask.DueDate.Should().Be(request.DueDate);
        createdTask.IsCompleted.Should().BeFalse();
    }

    [Fact(DisplayName = "Should return BadRequest for invalid request")]
    [Trait("Category", "Integration")]
    public async Task CreateTask_WithInvalidRequest_ReturnsBadRequest()
    {
        var invalidRequest = TaskFixtures.GetInvalidCreateTaskRequest();

        var response = await _client.PostAsync(BaseUrl, GetJsonContent(invalidRequest));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrWhiteSpace();
        content.Should().Contain("errors").And.Contain("Title").And.Contain("DueDate");
    }

    [Fact(DisplayName = "Should return user tasks")]
    [Trait("Category", "Integration")]
    public async Task GetUserTasks_WithExistingTasks_ReturnsTasks()
    {
        var request = TaskFixtures.GetValidCreateTaskRequest();
        var createResponse = await _client.PostAsync(BaseUrl, GetJsonContent(request));
        var createdTask = await DeserializeResponse<TaskItemResponse>(createResponse);

        var getResponse = await _client.GetAsync($"{BaseUrl}/{request.UserId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var tasks = await DeserializeResponse<List<TaskItemResponse>>(getResponse);
        tasks.Should().NotBeNullOrEmpty();
        tasks.Should().Contain(t => t.Id == createdTask.Id);
    }

    [Fact(DisplayName = "Should return empty list if no tasks")]
    [Trait("Category", "Integration")]
    public async Task GetUserTasks_WithNoTasks_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();

        var response = await _client.GetAsync($"{BaseUrl}/{userId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tasks = await DeserializeResponse<List<TaskItemResponse>>(response);
        tasks.Should().NotBeNull();
        tasks.Should().BeEmpty();
    }

    [Fact(DisplayName = "Should complete existing task")]
    [Trait("Category", "Integration")]
    public async Task CompleteTask_WithExistingTask_ReturnsNoContent()
    {
        var request = TaskFixtures.GetValidCreateTaskRequest();
        var createResponse = await _client.PostAsync(BaseUrl, GetJsonContent(request));
        var createdTask = await DeserializeResponse<TaskItemResponse>(createResponse);

        var completeResponse = await _client.PutAsync($"{BaseUrl}/{createdTask.Id}/complete", null);

        completeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"{BaseUrl}/{request.UserId}");
        var tasks = await DeserializeResponse<List<TaskItemResponse>>(getResponse);
        tasks.First(t => t.Id == createdTask.Id).IsCompleted.Should().BeTrue();
    }

    [Fact(DisplayName = "Should return NotFound if task does not exist (complete)")]
    [Trait("Category", "Integration")]
    public async Task CompleteTask_WithNonExistingTask_ReturnsNotFound()
    {
        var nonExistingId = Guid.NewGuid();

        var response = await _client.PutAsync($"{BaseUrl}/{nonExistingId}/complete", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "Should delete existing task")]
    [Trait("Category", "Integration")]
    public async Task DeleteTask_WithExistingTask_ReturnsNoContent()
    {
        var request = TaskFixtures.GetValidCreateTaskRequest();
        var createResponse = await _client.PostAsync(BaseUrl, GetJsonContent(request));
        var createdTask = await DeserializeResponse<TaskItemResponse>(createResponse);

        var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/{createdTask.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"{BaseUrl}/{request.UserId}");
        var tasks = await DeserializeResponse<List<TaskItemResponse>>(getResponse);
        tasks.Should().NotContain(t => t.Id == createdTask.Id);
    }

    [Fact(DisplayName = "Should return NotFound if task does not exist (delete)")]
    [Trait("Category", "Integration")]
    public async Task DeleteTask_WithNonExistingTask_ReturnsNotFound()
    {
        var nonExistingId = Guid.NewGuid();

        var response = await _client.DeleteAsync($"{BaseUrl}/{nonExistingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
