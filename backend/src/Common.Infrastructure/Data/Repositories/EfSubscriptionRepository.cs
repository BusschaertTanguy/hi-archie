using Core.Domain.Communities.Entities;
using Core.Domain.Communities.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.Data.Repositories;

internal sealed class EfSubscriptionRepository(AppDbContext dbContext) : ISubscriptionRepository
{
    public Task<bool> ExistsAsync(Guid communityId, Guid userId)
    {
        return dbContext.Set<Subscription>().AnyAsync(s => s.CommunityId == communityId && s.UserId == userId);
    }

    public Task AddAsync(Subscription subscription)
    {
        return dbContext.Set<Subscription>().AddAsync(subscription).AsTask();
    }

    public async Task<Subscription> GetByIdAsync(Guid communityId, Guid userId)
    {
        return await dbContext.Set<Subscription>().FindAsync(communityId, userId) ?? throw new InvalidOperationException("Subscription not found");
    }

    public Task DeleteAsync(Subscription subscription)
    {
        dbContext.Set<Subscription>().Remove(subscription);
        return Task.CompletedTask;
    }
}