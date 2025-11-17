using DotEnv.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Extensions;
using Playtesters.API.Middlewares;
using Playtesters.API.UseCases.Testers;
using SimpleResults;

var builder = WebApplication.CreateBuilder(args);
new EnvLoader().Load();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithApiKey();
builder.Services.AddUseCases();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=playtesters.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ApiKeyMiddleware>();
app.UseRequestLocalization("en");
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

app.Run();
