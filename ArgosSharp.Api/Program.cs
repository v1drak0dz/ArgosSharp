using ArgosSharp.Api.DependeciesInjection;
using ArgosSharp.Api.Middlewares;
using ArgosSharp.Application;
using ArgosSharp.Domain;
using ArgosSharp.Infrastructure;
using ArgosSharp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add Configurations from Environment
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Cors
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
{
    policy
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod();
}));

#region [Dependecies Injection]

builder.Services.AddHttpClient();
builder.Services.AddMiddlewareDependencies();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddDomain();
//builder.Services.AddValidators();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion [Dependencies Injection]

var app = builder.Build();

// Migrate
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ArgosDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("Frontend");

app.Run();
