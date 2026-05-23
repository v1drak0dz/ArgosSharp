using ArgosSharp.Api;
using ArgosSharp.Application.Services.JobStore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddInfrastructure();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var store = scope.ServiceProvider.GetRequiredService<IJobStore>();
await store.InitializeAsync();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
