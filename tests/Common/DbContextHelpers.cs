using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using System.Linq.Expressions;

namespace Playtesters.API.Tests.Common;

public partial class TestBase
{
    protected async Task<TEntity> FirstOrDefaultAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        using var scope = ApplicationFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
        return await dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
    }

    protected int Count<TEntity>() where TEntity : class
    {
        using var scope = ApplicationFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
        return dbContext.Set<TEntity>().Count();
    }

    protected async Task<List<TEntity>> ToListAsync<TEntity>() where TEntity : class
    {
        using var scope = ApplicationFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
        return await dbContext.Set<TEntity>().ToListAsync();
    }

    protected async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
    {
        using var scope = ApplicationFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetService<AppDbContext>();
        context.Add(entity);
        await context.SaveChangesAsync();
    }

    protected async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        using var scope = ApplicationFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetService<AppDbContext>();
        await context.AddRangeAsync(entities);
        await context.SaveChangesAsync();
    }
}
