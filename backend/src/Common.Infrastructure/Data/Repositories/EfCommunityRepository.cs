using Core.Domain.Communities.Entities;
using Core.Domain.Communities.Repositories;

namespace Common.Infrastructure.Data.Repositories;

internal sealed class EfCommunityRepository(AppDbContext dbContext) : ICommunityRepository
{
    public Task AddAsync(Community community)
    {
        return dbContext.Set<Community>().AddAsync(community).AsTask();
    }

    public async Task<Community> GetByIdAsync(Guid id)
    {
        return await dbContext.Set<Community>().FindAsync(id) ??
               throw new InvalidOperationException("Community not found");
    }

    public Task RemoveAsync(Community community)
    {
        dbContext.Set<Community>().Remove(community);
        return Task.CompletedTask;
    }
}