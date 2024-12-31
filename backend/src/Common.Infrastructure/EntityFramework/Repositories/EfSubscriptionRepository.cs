using Core.Domain.Communities.Entities;
using Core.Domain.Communities.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.EntityFramework.Repositories;

internal sealed class EfSubscriptionRepository(AppDbContext dbContext) : ISubscriptionRepository
{
    public Task<Subscription> GetByIdAsync(Guid communityId, Guid userId)
    {
        return dbContext.Set<Subscription>().AsNoTracking().FirstAsync(s => s.CommunityId == communityId && s.UserId == userId);
    }

    public Task<bool> ExistsAsync(Guid communityId, Guid userId)
    {
        return dbContext.Set<Subscription>().AnyAsync(s => s.CommunityId == communityId && s.UserId == userId);
    }

    public async Task AddAsync(Subscription subscription)
    {
        await dbContext.Set<Subscription>().AddAsync(subscription);
        await dbContext.SaveChangesAsync();
    }

    public Task RemoveAsync(Guid communityId, Guid userId)
    {
        return dbContext.Set<Subscription>()
            .Where(s => s.CommunityId == communityId && s.UserId == userId)
            .ExecuteDeleteAsync();
    }
}