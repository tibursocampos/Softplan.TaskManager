using FluentValidation.AspNetCore;
using FluentValidation;
using TaskManager.API.Models;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Services;
using TaskManager.Infra.Repositories;

namespace TaskManager.API.Extensions;

/// <summary>
/// Provides extension methods for registering application dependencies into the service container.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers application-level services, such as business logic handlers.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        return services;
    }

    /// <summary>
    /// Registers infrastructure-level services, such as repositories and data access implementations.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ITaskRepository, TaskRepository>();
        return services;
    }

    /// <summary>
    /// Registers FluentValidation services and validators for request validation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<CreateTaskRequestValidator>();
        return services;
    }
}
