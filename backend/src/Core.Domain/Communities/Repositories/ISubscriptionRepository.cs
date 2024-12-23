using Core.Domain.Communities.Entities;

namespace Core.Domain.Communities.Repositories;

public interface ISubscriptionRepository
{
    Task<bool> ExistsAsync(Guid communityId, Guid userId);
    Task AddAsync(Subscription subscription);
    Task<Subscription> GetByIdAsync(Guid communityId, Guid userId);
    Task RemoveAsync(Subscription subscription);
}