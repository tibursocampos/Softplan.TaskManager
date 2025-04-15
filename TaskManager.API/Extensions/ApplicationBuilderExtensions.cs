using TaskManager.API.Middleware;
using TaskManager.Infra.Helper;
using TaskManager.Infra;

namespace TaskManager.API.Extensions;

/// <summary>
/// Extension methods for configuring the application builder during startup.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures development-specific settings, such as Swagger UI.
    /// This should only run when the application is in the Development environment.
    /// </summary>
    /// <param name="app">The application builder instance.</param>
    /// <returns>The same application builder instance for chaining.</returns>
    public static IApplicationBuilder ConfigureDevelopmentSettings(this IApplicationBuilder app)
    {
        if (app is WebApplication webApp && webApp.Environment.IsDevelopment())
        {
            webApp.UseSwagger();
            webApp.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API v1");
            });
        }
        return app;
    }

    /// <summary>
    /// Configures the middleware pipeline by adding global exception handling, HTTPS redirection, and routing.
    /// </summary>
    /// <param name="app">The application builder instance.</param>
    /// <returns>The same application builder instance for chaining.</returns>
    public static IApplicationBuilder ConfigureMiddlewarePipeline(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.UseRouting();
        return app;
    }

    /// <summary>
    /// Seeds the database with initial data at application startup.
    /// </summary>
    /// <param name="app">The application builder instance.</param>
    /// <returns>The same application builder instance for chaining.</returns>
    public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TaskManagerContext>();
        SeedData.Initialize(context);
        return app;
    }
}
