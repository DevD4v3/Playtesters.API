using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using SimpleResults;

namespace Playtesters.API.UseCases.Testers;

public record DeleteTesterResponse(string Name, string AccessKey);

public class DeleteTesterUseCase(AppDbContext dbContext)
{
    public async Task<Result<DeleteTesterResponse>> ExecuteAsync(string name)
    {
        var tester = await dbContext
            .Set<Tester>()
            .FirstOrDefaultAsync(t => t.Name == name);

        if (tester is null)
            return Result.NotFound();

        dbContext.Remove(tester);
        await dbContext.SaveChangesAsync();

        var response = new DeleteTesterResponse(tester.Name, tester.AccessKey);
        return Result.Success(response);
    }
}
