using Core.Domain.Communities.Entities;

namespace Core.Domain.Communities.Repositories;

public interface ICommunityRepository
{
    Task AddAsync(Community community);
    Task<Community> GetByIdAsync(Guid id);
    Task RemoveAsync(Community community);
}