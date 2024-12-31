using Common.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.EntityFramework.Queries;

internal sealed class EfQueryProcessor(AppDbContext dbContext) : IQueryProcessor
{
    public IQueryable<T> Query<T>() where T : class
    {
        return dbContext.Set<T>().AsNoTracking();
    }
}