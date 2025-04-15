using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using TaskManager.Infra;

namespace TaskManager.API.Extensions;

/// <summary>
/// Provides extension methods for configuring common services in the API.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures API-related services such as controllers and endpoint discovery.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        return services;
    }

    /// <summary>
    /// Configures Swagger for API documentation generation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskManager API", Version = "v1" });
        });
        return services;
    }

    /// <summary>
    /// Configures the database context. Uses an in-memory database named "TaskManagerDB".
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TaskManagerContext>(options =>
            options.UseInMemoryDatabase("TaskManagerDB"));
        return services;
    }

    /// <summary>
    /// Configures logging using Serilog.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddSerilog();
        });
        return services;
    }
}
