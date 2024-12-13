using Core.Domain.Communities.Entities;
using Core.Domain.Communities.Repositories;

namespace Common.Infrastructure.Data.Repositories;

internal sealed class EfCommunityRepository(AppDbContext dbContext) : ICommunityRepository
{
    public Task AddAsync(Community community)
    {
        return dbContext.Set<Community>().AddAsync(community).AsTask();
    }
}