using TaskManager.API.Middleware;
using TaskManager.Infra.Helper;
using TaskManager.Infra;

namespace TaskManager.API.Extensions;

public static class ApplicationBuilderExtensions
{
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

    public static IApplicationBuilder ConfigureMiddlewarePipeline(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.UseRouting();
        return app;
    }

    public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TaskManagerContext>();
        SeedData.Initialize(context);
        return app;
    }
}
