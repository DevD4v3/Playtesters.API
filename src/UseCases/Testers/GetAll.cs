using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using Playtesters.API.Extensions;
using SimpleResults;

namespace Playtesters.API.UseCases.Testers;

public enum GetTestersOrderBy
{
    CreatedAtDesc,
    CreatedAtAsc,
    TotalHoursPlayedDesc,
    TotalHoursPlayedAsc
}

public record GetTestersRequest(string OrderBy);

public class GetTestersResponse
{
    public required string Name { get; init; }
    public required string AccessKey { get; init; }
    public required double TotalHoursPlayed { get; init; }
    public required string TotalPlaytime { get; init; }
    public required string CreatedAt { get; init; }
}

public class GetTestersValidator : AbstractValidator<GetTestersRequest>
{
    private static readonly string[] s_allowed = Enum.GetNames<GetTestersOrderBy>();

    public GetTestersValidator()
    {
        RuleFor(t => t.OrderBy)
            .Must(BeValidOrderBy)
            .WithMessage(t =>
                $"Invalid orderBy '{t.OrderBy}'. Allowed values: {string.Join(", ", s_allowed)}");
    }

    private bool BeValidOrderBy(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        return Enum.TryParse<GetTestersOrderBy>(value, ignoreCase: true, out _);
    }
}

public class GetTestersUseCase(
    AppDbContext dbContext,
    GetTestersValidator validator)
{
    public async Task<ListedResult<GetTestersResponse>> ExecuteAsync(GetTestersRequest request)
    {
        var validationResult = validator.Validate(request);
        if (validationResult.IsFailed())
            return validationResult.Invalid();

        var orderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? 
            GetTestersOrderBy.CreatedAtDesc : 
            Enum.Parse<GetTestersOrderBy>(request.OrderBy, ignoreCase: true);

        var query = dbContext.Set<Tester>().AsQueryable();
        query = orderBy switch
        {
            GetTestersOrderBy.CreatedAtAsc => query.OrderBy(t => t.CreatedAt),
            GetTestersOrderBy.CreatedAtDesc => query.OrderByDescending(t => t.CreatedAt),
            GetTestersOrderBy.TotalHoursPlayedAsc => query.OrderBy(t => t.TotalHoursPlayed),
            GetTestersOrderBy.TotalHoursPlayedDesc => query.OrderByDescending(t => t.TotalHoursPlayed),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };

        var testers = await query
            .Select(t => new GetTestersResponse
            {
                Name = t.Name,
                AccessKey = t.AccessKey,
                TotalHoursPlayed = t.TotalHoursPlayed,
                TotalPlaytime = t.TotalHoursPlayed.ToHhMmSs(),
                CreatedAt = t.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToListAsync();

        return Result.Success(testers);
    }
}
