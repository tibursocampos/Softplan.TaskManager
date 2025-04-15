namespace TaskManager.API.Tests.Integration.Config;

/// <summary>
/// Factory for bootstrapping the application in memory for integration tests.
/// </summary>
/// <remarks>
/// Configures an isolated environment with:
/// <list type="bullet">
///   <item><description>In-memory database (unique per test run)</description></item>
///   <item><description>Overridden service configurations for testing</description></item>
/// </list>
/// 
/// Key behaviors:
/// <list type="bullet">
///   <item><description>Replaces the production database with an in-memory version</description></item>
///   <item><description>Ensures database schema is created before tests run</description></item>
///   <item><description>Generates unique database names to prevent test collisions</description></item>
/// </list>
/// </remarks>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Configures test-specific services and environment.
    /// </summary>
    /// <param name="builder">The web host builder to configure</param>
    /// <remarks>
    /// The implementation:
    /// 1. Removes any existing DB context configuration
    /// 2. Registers a new in-memory database context
    /// 3. Ensures the database schema is created
    /// </remarks>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TaskManagerContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            var dbName = $"InMemoryDbForTesting_{Guid.NewGuid()}";
            services.AddDbContext<TaskManagerContext>(options =>
            {
                options.UseInMemoryDatabase(dbName);
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<TaskManagerContext>();

            db.Database.EnsureCreated();
        });
    }
}
