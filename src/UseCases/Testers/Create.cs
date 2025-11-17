using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using SimpleResults;

namespace Playtesters.API.UseCases.Testers;

public record CreateTesterRequest(string Name);
public record CreateTesterResponse(string Name, string AccessKey);

public class CreateTesterValidator
    : AbstractValidator<CreateTesterRequest>
{
    public CreateTesterValidator()
    {
        RuleFor(t => t.Name)
            .NotEmpty()
            .MinimumLength(3);
    }
}

public class CreateTesterUseCase(
    AppDbContext dbContext,
    CreateTesterValidator validator)
{
    public async Task<Result<CreateTesterResponse>> ExecuteAsync(CreateTesterRequest request)
    {
        var validationResult = validator.Validate(request);
        if (validationResult.IsFailed())
            return validationResult.Invalid();

        bool exists = await dbContext
            .Set<Tester>()
            .AnyAsync(t => t.Name == request.Name);

        if (exists)
            return Result.Conflict("Username already exists.");

        var tester = new Tester
        {
            Name = request.Name
        };
        var response = new CreateTesterResponse(request.Name, tester.AccessKey);
        dbContext.Add(tester);

        await dbContext.SaveChangesAsync();
        return Result.Success(response);
    }
}
