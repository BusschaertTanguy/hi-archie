using Core.Domain.Communities.Entities;

namespace Core.Domain.Communities.Repositories;

public interface ICommunityRepository
{
    Task<Community> GetByIdAsync(Guid id);
    Task AddAsync(Community community);
    Task UpdateAsync(Community community);
    Task RemoveAsync(Guid id);
}