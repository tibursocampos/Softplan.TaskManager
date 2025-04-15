namespace TaskManager.Integration.Tests.Config;

/// <summary>
/// Base class for API integration tests providing common utilities and configured test dependencies.
/// </summary>
/// <remarks>
/// This abstract class initializes:
/// <list type="bullet">
///   <item><description>Pre-configured HttpClient with JSON support</description></item>
///   <item><description>DbContext for direct database access</description></item>
///   <item><description>Consistent JSON serialization settings</description></item>
/// </list>
/// </remarks>
public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>
{
    /// <summary>
    /// Pre-configured HttpClient with default Accept header for JSON
    /// </summary>
    /// <remarks>
    /// Configured to send "Accept: application/json" by default.
    /// The client is automatically disposed when the test class is disposed.
    /// </remarks>
    protected readonly HttpClient _client;

    /// <summary>
    /// Shared JSON serialization options configured for:
    /// - CamelCase property names
    /// - Case-insensitive property matching
    /// </summary>
    protected readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Database context for direct database access in tests
    /// </summary>
    /// <remarks>
    /// Uses a scoped instance from the test server's DI container.
    /// Each test class gets a fresh in-memory database instance.
    /// </remarks>
    protected readonly TaskManagerContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the test class with dependencies from the factory
    /// </summary>
    /// <param name="factory">The test web application factory</param>
    protected BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerContext>();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <summary>
    /// Serializes an object to JSON StringContent with the configured serialization settings
    /// </summary>
    /// <param name="obj">The object to serialize</param>
    /// <returns>StringContent with JSON representation</returns>
    /// <example>
    /// var content = GetJsonContent(new { Name = "Test" });
    /// var response = await _client.PostAsync("/api/endpoint", content);
    /// </example>
    protected StringContent GetJsonContent(object obj)
    {
        return new StringContent(
            JsonSerializer.Serialize(obj, _jsonOptions),
            Encoding.UTF8,
            "application/json");
    }

    /// <summary>
    /// Deserializes the HTTP response content to the specified type
    /// </summary>
    /// <typeparam name="T">Target type for deserialization</typeparam>
    /// <param name="response">HTTP response to deserialize</param>
    /// <returns>Deserialized object of type T</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when:
    /// <list type="bullet">
    ///   <item><description>Response content is null or empty</description></item>
    ///   <item><description>Deserialization returns null</description></item>
    /// </list>
    /// </exception>
    /// <example>
    /// var response = await _client.GetAsync("/api/items");
    /// var items = await DeserializeResponse<List<Item>>(response);
    /// </example>
    protected async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException("Response content is null or empty");

        return JsonSerializer.Deserialize<T>(content, _jsonOptions)
               ?? throw new InvalidOperationException("Deserialization returned null");
    }
}
