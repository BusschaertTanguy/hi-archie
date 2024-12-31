using Core.Domain.Posts.Entities;
using Core.Domain.Posts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.EntityFramework.Repositories;

internal sealed class EfPostVoteRepository(AppDbContext dbContext) : IPostVoteRepository
{
    public Task<PostVote?> GetByIdAsync(Guid postId, Guid userId)
    {
        return dbContext.Set<PostVote>().AsNoTracking().FirstOrDefaultAsync(pv => pv.PostId == postId && pv.UserId == userId);
    }

    public async Task AddAsync(PostVote vote)
    {
        await dbContext.Set<PostVote>().AddAsync(vote);
        await dbContext.SaveChangesAsync();
    }

    public Task UpdateAsync(PostVote vote)
    {
        dbContext.Set<PostVote>().Update(vote);
        return dbContext.SaveChangesAsync();
    }

    public Task RemoveAsync(Guid postId, Guid userId)
    {
        return dbContext.Set<PostVote>()
            .Where(pv => pv.PostId == postId && pv.UserId == userId)
            .ExecuteDeleteAsync();
    }
}