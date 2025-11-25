using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using SimpleResults;

namespace Playtesters.API.UseCases.Testers;

public record UpdatePlaytimeRequest(double HoursPlayed);

public class UpdatePlaytimeValidator
    : AbstractValidator<UpdatePlaytimeRequest>
{
    public UpdatePlaytimeValidator()
    {
        RuleFor(t => t.HoursPlayed)
            .GreaterThanOrEqualTo(0);
    }
}

public class UpdatePlaytimeUseCase(
    AppDbContext dbContext,
    UpdatePlaytimeValidator validator)
{
    public async Task<Result> ExecuteAsync(string accessKey, UpdatePlaytimeRequest request)
    {
        var validationResult = validator.Validate(request);
        if (validationResult.IsFailed())
            return validationResult.Invalid();

        double roundedHours = Math.Round(request.HoursPlayed, 2);
        int affectedRows = await dbContext
            .Set<Tester>()
            .Where(t => t.AccessKey == accessKey)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.TotalHoursPlayed, t => t.TotalHoursPlayed + roundedHours));

        return affectedRows == 0 ? Result.NotFound() : Result.Success();
    }
}
