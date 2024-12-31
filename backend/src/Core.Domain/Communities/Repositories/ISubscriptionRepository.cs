using Core.Domain.Communities.Entities;

namespace Core.Domain.Communities.Repositories;

public interface ISubscriptionRepository
{
    Task<bool> ExistsAsync(Guid communityId, Guid userId);
    Task<Subscription> GetByIdAsync(Guid communityId, Guid userId);
    Task AddAsync(Subscription subscription);
    Task RemoveAsync(Guid communityId, Guid userId);
}