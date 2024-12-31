using Core.Domain.Comments.Entities;
using Core.Domain.Comments.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.EntityFramework.Repositories;

internal sealed class EfCommentVoteRepository(AppDbContext dbContext) : ICommentVoteRepository
{
    public Task<CommentVote?> GetByIdAsync(Guid commentId, Guid userId)
    {
        return dbContext.Set<CommentVote>().AsNoTracking().FirstOrDefaultAsync(cv => cv.CommentId == commentId && cv.UserId == userId);
    }

    public async Task AddAsync(CommentVote vote)
    {
        await dbContext.Set<CommentVote>().AddAsync(vote);
        await dbContext.SaveChangesAsync();
    }

    public Task UpdateAsync(CommentVote vote)
    {
        dbContext.Set<CommentVote>().Update(vote);
        return dbContext.SaveChangesAsync();
    }

    public Task RemoveAsync(Guid commentId, Guid userId)
    {
        return dbContext.Set<CommentVote>()
            .Where(cv => cv.CommentId == commentId && cv.UserId == userId)
            .ExecuteDeleteAsync();
    }
}