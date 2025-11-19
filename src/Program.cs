using DotEnv.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.ExceptionHandlers;
using Playtesters.API.Extensions;
using Playtesters.API.Middlewares;
using Playtesters.API.UseCases.TesterAccessHistory;
using Playtesters.API.UseCases.Testers;
using SimpleResults;

var builder = WebApplication.CreateBuilder(args);
var envVars = new EnvLoader().Load();
var dataSource = envVars["SQLITE_DATA_SOURCE"] ?? "playtesters.db";

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithApiKey();
builder.Services.AddUseCases();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dataSource}"));

var app = builder.Build();
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

var testerGroup = app
    .MapGroup("/api/testers")
    .WithTags("Tester")
    .WithOpenApi();

testerGroup.MapPost("/", async (
    [FromBody]CreateTesterRequest request,
    CreateTesterUseCase useCase) =>
{
    var response = await useCase.ExecuteAsync(request);
    return response.ToHttpResult();
})
.Produces<Result<CreateTesterResponse>>();

testerGroup.MapPut("/{name}", async (
    string name, 
    [FromBody]UpdateTesterRequest request, 
    UpdateTesterUseCase useCase) =>
{
    var response = await useCase.ExecuteAsync(name, request);
    return response.ToHttpResult();
})
.Produces<Result<UpdateTesterResponse>>();

testerGroup.MapGet("/", async ([FromServices]GetTestersUseCase useCase) =>
{
    var response = await useCase.ExecuteAsync();
    return response.ToHttpResult();
})
.Produces<ListedResult<GetTestersResponse>>();

testerGroup.MapPost("/validate-access", async (
    [FromBody]ValidateTesterAccessRequest request,
    ValidateTesterAccessUseCase useCase) =>
{
    var response = await useCase.ExecuteAsync(request);
    return response.ToHttpResult();
})
.Produces<Result<ValidateTesterAccessResponse>>();

testerGroup.MapGet("/access-history", async (
    [AsParameters]GetAllTestersAccessHistoryRequest request, 
    GetAllTestersAccessHistoryUseCase useCase) =>
{
    var response = await useCase.ExecuteAsync(request);
    return response.ToHttpResult();
})
.Produces<PagedResult<GetAllTestersAccessHistoryResponse>>();

app.Run();
