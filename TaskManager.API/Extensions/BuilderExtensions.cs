using Serilog;

namespace TaskManager.API.Extensions;

/// <summary>
/// Extension methods for configuring the application host builder.
/// </summary>
public static class BuilderExtensions
{
    /// <summary>
    /// Configures Serilog as the logging provider for the application.
    /// This method reads configuration from appsettings and enriches logs with context and properties.
    /// </summary>
    /// <param name="host">The host builder instance.</param>
    /// <returns>The same host builder instance for chaining.</returns>
    public static IHostBuilder AddSerilogConfiguration(this IHostBuilder host)
    {
        host.UseSerilog((context, config) =>
        {
            config
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "TaskManager.API")
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}");
        });
        return host;
    }
}
