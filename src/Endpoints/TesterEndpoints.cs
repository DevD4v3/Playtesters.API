using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Playtesters.API.UseCases.TesterAccessHistory;
using Playtesters.API.UseCases.Testers;
using SimpleResults;

namespace Playtesters.API.Endpoints;

public static class TesterEndpoints
{
    public static void MapTesterEndpoints(this WebApplication app)
    {
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

        testerGroup.MapPatch("/{name}", async (
            string name,
            [FromBody]UpdateTesterRequest request,
            UpdateTesterUseCase useCase) =>
        {
            var response = await useCase.ExecuteAsync(name, request);
            return response.ToHttpResult();
        })
        .Produces<Result<UpdateTesterResponse>>();

        testerGroup.MapPatch("/{accessKey}/playtime", async (
            string accessKey,
            [FromBody]UpdatePlaytimeRequest request,
            UpdatePlaytimeUseCase useCase) =>
        {
            var response = await useCase.ExecuteAsync(accessKey, request);
            return response.ToHttpResult();
        })
        .Produces<Result>()
        .WithMetadata(new AllowAnonymousAttribute());

        testerGroup.MapGet("/", async (
            [AsParameters]GetTestersRequest request,
            [FromServices]GetTestersUseCase useCase) =>
        {
            var response = await useCase.ExecuteAsync(request);
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
        .Produces<Result<ValidateTesterAccessResponse>>()
        .WithMetadata(new AllowAnonymousAttribute());

        testerGroup.MapGet("/access-history", async (
            [AsParameters]GetAllTestersAccessHistoryRequest request,
            GetAllTestersAccessHistoryUseCase useCase) =>
        {
            var response = await useCase.ExecuteAsync(request);
            return response.ToHttpResult();
        })
        .Produces<PagedResult<GetAllTestersAccessHistoryResponse>>();

        testerGroup.MapPost("/revoke-all-keys", async (
            [FromServices]RevokeAllKeysUseCase useCase) =>
        {
            var response = await useCase.ExecuteAsync();
            return response.ToHttpResult();
        })
        .Produces<Result<RevokeAllKeysResponse>>();
    }
}
