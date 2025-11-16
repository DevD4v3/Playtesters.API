using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using SimpleResults;

namespace Playtesters.API.UseCases.Testers;

public class GetTestersResponse
{
    public required string UserName { get; init; }
    public required string AccessKey { get; init; }
    public required string CreatedAt { get; init; }
}

public class GetTestersUseCase(AppDbContext dbContext)
{
    public async Task<ListedResult<GetTestersResponse>> ExecuteAsync()
    {
        var testers = await dbContext.Set<Tester>()
            .Select(t => new GetTestersResponse
            {
                UserName = t.UserName,
                AccessKey = t.AccessKey,
                CreatedAt = t.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToListAsync();

        return Result.Success(testers);
    }
}
