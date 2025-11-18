using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using SimpleResults;
using System.Net;

namespace Playtesters.API.UseCases.Testers;

public record ValidateTesterAccessRequest(string Name, string AccessKey, string IpAddress);
public record ValidateTesterAccessResponse(string Name);

public class ValidateTesterAccessValidator 
    : AbstractValidator<ValidateTesterAccessRequest>
{
    public ValidateTesterAccessValidator()
    {
        RuleFor(t => t.Name)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(t => t.AccessKey)
            .NotEmpty()
            .Must(key => Guid.TryParse(key, out _))
            .WithMessage("Access Key must be a valid GUID.");

        RuleFor(t => t.IpAddress)
            .NotEmpty()
            .Must(ip => IPAddress.TryParse(ip, out _))
            .WithMessage("IpAddress must be a valid IPv4 or IPv6 address.");
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
            .FirstOrDefaultAsync(t => t.Name == request.Name);

        if (tester is null || tester.AccessKey != request.AccessKey)
            return Result.Invalid("Invalid credentials.");

        var history = new AccessValidationHistory
        {
            TesterId = tester.Id,
            IpAddress = request.IpAddress
        };
        dbContext.Add(history);
        await dbContext.SaveChangesAsync();

        var response = new ValidateTesterAccessResponse(tester.Name);
        return Result.Success(response);
    }
}
