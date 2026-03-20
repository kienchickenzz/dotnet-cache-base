using BaseCache.Api.Configurations;
using BaseCache.Api.Extensions;
using BaseCache.Api.OpenApi;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);

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

var app = builder.Build();

// Initialize database (migrate + seed)
await app.Services.InitializeDatabaseAsync();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseSwaggerExtension();

app.UseRouting();
// app.UseHttpsRedirection(); // Disable for dev

app.UseAuthorization();
app.MapControllers();


app.Run();
