using Common.Application.Commands;

namespace Common.Infrastructure.Data.Commands;

internal sealed class EfUnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public Task CommitAsync()
    {
        return dbContext.SaveChangesAsync();
    }
}