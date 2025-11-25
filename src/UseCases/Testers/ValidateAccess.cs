using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using Playtesters.API.Extensions;
using Playtesters.API.Services;
using SimpleResults;

namespace Playtesters.API.UseCases.Testers;

public record ValidateTesterAccessRequest(string AccessKey);
public record ValidateTesterAccessResponse(
    string Name, 
    double TotalHoursPlayed,
    string TotalPlaytime
);

public class ValidateTesterAccessValidator 
    : AbstractValidator<ValidateTesterAccessRequest>
{
    public ValidateTesterAccessValidator()
    {
        RuleFor(t => t.AccessKey)
            .NotEmpty()
            .Must(key => Guid.TryParse(key, out _))
            .WithMessage("Access Key must be a valid GUID.");
    }
}

public class ValidateTesterAccessUseCase(
    AppDbContext dbContext,
    ValidateTesterAccessValidator validator,
    IClientIpProvider clientIpProvider,
    IIpGeoLocationService ipGeoLocationService)
{
    public async Task<Result<ValidateTesterAccessResponse>> ExecuteAsync(
        ValidateTesterAccessRequest request)
    {
        var validationResult = validator.Validate(request);
        if (validationResult.IsFailed())
            return validationResult.Invalid();

        var tester = await dbContext
            .Set<Tester>()
            .FirstOrDefaultAsync(t => t.AccessKey == request.AccessKey);

        if (tester is null)
            return Result.Invalid("Invalid credentials.");

        var ipAddress = clientIpProvider.GetClientIp();
        var location = await ipGeoLocationService.GetLocationAsync(ipAddress);
        var accessHistory = new AccessValidationHistory
        {
            TesterId = tester.Id,
            IpAddress = ipAddress,
            Country = location.Country,
            City = location.City
        };
        dbContext.Add(accessHistory);
        await dbContext.SaveChangesAsync();

        var response = new ValidateTesterAccessResponse(
            Name: tester.Name, 
            tester.TotalHoursPlayed,
            tester.TotalHoursPlayed.ToHhMmSs()
        );
        return Result.Success(response);
    }
}
