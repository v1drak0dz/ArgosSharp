using ArgosSharp.Api;
using ArgosSharp.Api.DependeciesInjection;
using ArgosSharp.Infrastructure.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

#region [Dependecies Injection]

builder.Services.AddInfrastructure();
builder.Services.AddJobDependencies();
builder.Services.AddValidators();

#endregion [Dependencies Injection]

var app = builder.Build();

using var scope = app.Services.CreateScope();
var store = scope.ServiceProvider.GetRequiredService<JobUnitOfWork>();
await store.InitializeAsync();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
