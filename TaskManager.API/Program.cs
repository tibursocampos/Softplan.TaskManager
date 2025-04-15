using TaskManager.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiConfiguration()
    .AddSwaggerConfiguration()
    .AddDatabaseConfiguration(builder.Configuration)
    .AddLoggingConfiguration(builder.Configuration);

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices()
    .AddValidationServices();

builder.Host.AddSerilogConfiguration();

var app = builder.Build();

app.UseCorrelationId();

app.ConfigureDevelopmentSettings()
   .ConfigureMiddlewarePipeline()
   .SeedDatabase();

app.MapControllers();

await app.RunAsync();

public partial class Program { }
