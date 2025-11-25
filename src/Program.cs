using DotEnv.Core;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Endpoints;
using Playtesters.API.ExceptionHandlers;
using Playtesters.API.Extensions;
using Playtesters.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var envVars = new EnvLoader().Load();
var dataSource = envVars["SQLITE_DATA_SOURCE"] ?? "playtesters.db";

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithApiKey();
builder.Services.AddServices();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dataSource}"));

var app = builder.Build();
await app.MigrateDatabaseAsync();
app.UseRequestLocalization("en");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/");
}

app.UseMiddleware<ApiKeyMiddleware>();
app.UseHttpsRedirection();
app.MapTesterEndpoints();
app.Run();

public partial class Program { }
