using Core.Domain.Posts.Entities;
using Core.Domain.Posts.Repositories;

namespace Common.Infrastructure.Data.Repositories;

internal sealed class EfPostVoteRepository(AppDbContext dbContext) : IPostVoteRepository
{
    public Task<PostVote?> GetByIdAsync(Guid postId, Guid userId)
    {
        return dbContext.Set<PostVote>().FindAsync(postId, userId).AsTask();
    }

    public Task AddAsync(PostVote vote)
    {
        return dbContext.Set<PostVote>().AddAsync(vote).AsTask();
    }

    public Task RemoveAsync(PostVote vote)
    {
        dbContext.Set<PostVote>().Remove(vote);
        return Task.CompletedTask;
    }
}