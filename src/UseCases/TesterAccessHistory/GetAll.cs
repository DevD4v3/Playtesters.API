using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using SimpleResults;

namespace Playtesters.API.UseCases.TesterAccessHistory;

public record GetAllTestersAccessHistoryRequest(
    string Name,
    string IpAddress,
    string Country,
    string FromDate,
    string ToDate,
    int PageNumber = 1,
    int PageSize = 20
);

public class GetAllTestersAccessHistoryResponse
{
    public required string Name { get; init; }
    public required string CheckedAt { get; init; }
    public required string IpAddress { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
}

public class GetAllTestersAccessHistoryValidator 
    : AbstractValidator<GetAllTestersAccessHistoryRequest>
{
    public GetAllTestersAccessHistoryValidator()
    {
        RuleFor(t => t.PageNumber).GreaterThan(0);
        RuleFor(t => t.PageSize).InclusiveBetween(10, 100);

        RuleFor(t => t.FromDate)
            .Must(IsValidDate)
            .WithMessage("FromDate must follow format 'yyyy-MM-dd'");

        RuleFor(t => t.ToDate)
            .Must(IsValidDate)
            .WithMessage("ToDate must follow format 'yyyy-MM-dd'");

        RuleFor(t => t)
            .Must(request =>
            {
                var from = DateOnly.Parse(request.FromDate);
                var to = DateOnly.Parse(request.ToDate);
                return from <= to;
            })
            .WithMessage("FromDate must be earlier than or equal to ToDate.")
            .When(t =>
                !string.IsNullOrWhiteSpace(t.FromDate) &&
                !string.IsNullOrWhiteSpace(t.ToDate) &&
                IsValidDate(t.FromDate) &&
                IsValidDate(t.ToDate));
    }

    private bool IsValidDate(string value)
        => string.IsNullOrWhiteSpace(value) || 
        DateOnly.TryParseExact(value, format: "yyyy-MM-dd", out _);
}

public class GetAllTestersAccessHistoryUseCase(
    AppDbContext dbContext,
    GetAllTestersAccessHistoryValidator validator)
{
    public async Task<PagedResult<GetAllTestersAccessHistoryResponse>> ExecuteAsync(
        GetAllTestersAccessHistoryRequest request)
    {
        var validationResult = validator.Validate(request);
        if (validationResult.IsFailed())
            return validationResult.Invalid();

        var query = dbContext
            .Set<AccessValidationHistory>()
            .Include(h => h.Tester)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(h => h.Tester.Name == request.Name);
        }

        if (!string.IsNullOrWhiteSpace(request.IpAddress))
        {
            query = query.Where(h => h.IpAddress == request.IpAddress);
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            query = query.Where(h => h.Country == request.Country);
        }

        if (!string.IsNullOrWhiteSpace(request.FromDate))
        {
            // Filter by the start date (FromDate).
            // Normalize to midnight (00:00:00) to include all records from that day.
            DateTime fromDate = DateTime.Parse(request.FromDate).Date;
            query = query.Where(h => h.CheckedAt >= fromDate);
        }

        if (!string.IsNullOrWhiteSpace(request.ToDate))
        {
            // Filter by the end date (ToDate).
            // Convert the date to the last possible moment of the day (23:59:59.9999999)
            // to ensure no records from the specified day are excluded.
            DateTime toDate = DateOnly
                .Parse(request.ToDate)
                .ToDateTime(TimeOnly.MaxValue);
            query = query.Where(h => h.CheckedAt <= toDate);
        }

        int itemsToSkip = (request.PageNumber - 1) * request.PageSize;
        var testers = await query
            .OrderByDescending(h => h.CheckedAt)
            .Select(h => new GetAllTestersAccessHistoryResponse
            {
                Name = h.Tester.Name,
                CheckedAt = h.CheckedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                IpAddress = h.IpAddress,
                Country = h.Country,
                City = h.City
            })
            .Skip(itemsToSkip)
            .Take(request.PageSize) 
            .ToListAsync();

        int totalRecords = await query.CountAsync();
        var pagedInfo = new PagedInfo(
            request.PageNumber, 
            request.PageSize,
            totalRecords
        );
        return Result.Success(testers, pagedInfo);
    }
}
