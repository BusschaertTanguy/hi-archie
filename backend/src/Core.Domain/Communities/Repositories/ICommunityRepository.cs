using Core.Domain.Communities.Entities;

namespace Core.Domain.Communities.Repositories;

public interface ICommunityRepository
{
    Task AddAsync(Community community);
    Task<Community> GetById(Guid id);
}