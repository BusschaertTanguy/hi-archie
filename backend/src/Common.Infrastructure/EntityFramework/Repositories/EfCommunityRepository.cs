using Core.Domain.Communities.Entities;
using Core.Domain.Communities.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.EntityFramework.Repositories;

internal sealed class EfCommunityRepository(AppDbContext dbContext) : ICommunityRepository
{
    public Task<Community> GetByIdAsync(Guid id)
    {
        return dbContext.Set<Community>().AsNoTracking().FirstAsync(c => c.Id == id);
    }

    public async Task AddAsync(Community community)
    {
        await dbContext.Set<Community>().AddAsync(community);
        await dbContext.SaveChangesAsync();
    }

    public Task UpdateAsync(Community community)
    {
        dbContext.Set<Community>().Update(community);
        return dbContext.SaveChangesAsync();
    }

    public Task RemoveAsync(Guid id)
    {
        return dbContext.Set<Community>()
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();
    }
}