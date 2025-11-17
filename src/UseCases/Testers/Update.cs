using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using SimpleResults;

namespace Playtesters.API.UseCases.Testers;

public record UpdateTesterRequest(string AccessKey);
public record UpdateTesterResponse(string Name, string AccessKey);

public class UpdateTesterValidator 
    : AbstractValidator<UpdateTesterRequest>
{
    public UpdateTesterValidator()
    {
        RuleFor(t => t.AccessKey)
            .Must(key => string.IsNullOrEmpty(key) || Guid.TryParse(key, out _))
            .WithMessage("Access Key must be a valid GUID.");
    }
}

public class UpdateTesterUseCase(
    AppDbContext dbContext,
    UpdateTesterValidator validator)
{
    public async Task<Result<UpdateTesterResponse>> ExecuteAsync(string name, UpdateTesterRequest request)
    {
        var validationResult = validator.Validate(request);
        if (validationResult.IsFailed())
            return validationResult.Invalid();

        var tester = await dbContext
            .Set<Tester>()
            .FirstOrDefaultAsync(t => t.Name == name);

        if (tester is null)
            return Result.NotFound();

        tester.AccessKey = request.AccessKey;
        await dbContext.SaveChangesAsync();

        var response = new UpdateTesterResponse(tester.Name, tester.AccessKey);
        return Result.Success(response);
    }
}
