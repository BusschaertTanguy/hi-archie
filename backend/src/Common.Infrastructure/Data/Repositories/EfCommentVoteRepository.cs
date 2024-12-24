using Core.Domain.Comments.Entities;
using Core.Domain.Comments.Repositories;

namespace Common.Infrastructure.Data.Repositories;

internal sealed class EfCommentVoteRepository(AppDbContext dbContext) : ICommentVoteRepository
{
    public Task<CommentVote?> GetByIdAsync(Guid commentId, Guid userId)
    {
        return dbContext.Set<CommentVote>().FindAsync(commentId, userId).AsTask();
    }

    public Task AddAsync(CommentVote vote)
    {
        return dbContext.Set<CommentVote>().AddAsync(vote).AsTask();
    }

    public Task RemoveAsync(CommentVote vote)
    {
        dbContext.Set<CommentVote>().Remove(vote);
        return Task.CompletedTask;
    }
}