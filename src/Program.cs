using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Extensions;
using Playtesters.API.UseCases.Testers;
using SimpleResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

app.UseRequestLocalization("en");
app.UseHttpsRedirection();

app.MapPost("/tester", async ([FromBody]CreateTesterRequest request, CreateTesterUseCase useCase) =>
{
    var response = await useCase.ExecuteAsync(request);
    return response.ToHttpResult();
})
.WithName("Testers")
.Produces<Result<CreateTesterResponse>>()
.WithOpenApi();

app.Run();
