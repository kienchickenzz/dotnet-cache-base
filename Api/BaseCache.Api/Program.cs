using BaseCache.Application;
using BaseCache.Infrastructure;
using BaseCache.Persistence;
using BaseCache.Persistence.Initialization;
using BaseCache.Api.Configurations;
using BaseCache.Api.Extensions;
using BaseCache.Api.OpenApi;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();

builder.AddConfigurations();
builder.Host.UseSerilogFromSettings();

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddApiServices();
builder.Services.AddApplication();

builder.Services.AddInfrastructurePersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Initialize database (migrate + seed)
await app.Services.InitializeDatabaseAsync();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwaggerExtension();
// }

app.UseSwaggerExtension();

app.UseRouting();
// app.UseHttpsRedirection(); // Disable for dev

app.UseAuthorization();
app.MapControllers();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var urls = string.Join(", ", app.Urls);
    logger.LogInformation("----------");
    logger.LogInformation("Application started successfully!");
    logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
    logger.LogInformation("Listening on: {Urls}", urls);
    logger.LogInformation("Swagger UI: {Urls}/swagger", app.Urls.FirstOrDefault());
    logger.LogInformation("----------");
});

app.Run();


public partial class Program { }
