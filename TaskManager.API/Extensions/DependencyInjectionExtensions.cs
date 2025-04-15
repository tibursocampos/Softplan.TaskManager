using FluentValidation.AspNetCore;
using FluentValidation;
using TaskManager.API.Models;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Services;
using TaskManager.Infra.Repositories;

namespace TaskManager.API.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ITaskRepository, TaskRepository>();
        return services;
    }

    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<CreateTaskRequestValidator>();
        return services;
    }
}
