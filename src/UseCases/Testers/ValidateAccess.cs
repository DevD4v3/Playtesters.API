using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using SimpleResults;

namespace Playtesters.API.UseCases.Testers;

public record ValidateTesterAccessRequest(string UserName, string AccessKey);
public record ValidateTesterAccessResponse(string UserName);

public class ValidateTesterAccessValidator 
    : AbstractValidator<ValidateTesterAccessRequest>
{
    public ValidateTesterAccessValidator()
    {
        RuleFor(t => t.UserName)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(t => t.AccessKey)
            .NotEmpty()
            .Must(key => Guid.TryParse(key, out _))
            .WithMessage("Access Key must be a valid GUID.");
    }
}

public class ValidateTesterAccessUseCase(
    AppDbContext dbContext,
    ValidateTesterAccessValidator validator)
{
    public async Task<Result<ValidateTesterAccessResponse>> ExecuteAsync(
        ValidateTesterAccessRequest request)
    {
        var validationResult = validator.Validate(request);
        if (validationResult.IsFailed())
            return validationResult.Invalid();

        var tester = await dbContext
            .Set<Tester>()
            .FirstOrDefaultAsync(t => t.UserName == request.UserName);

        if (tester is null || tester.AccessKey != request.AccessKey)
            return Result.Unauthorized("Invalid credentials.");

        var response = new ValidateTesterAccessResponse(tester.UserName);
        return Result.Success(response);
    }
}
