using Serilog;

namespace TaskManager.API.Extensions;

public static class BuilderExtensions
{
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
