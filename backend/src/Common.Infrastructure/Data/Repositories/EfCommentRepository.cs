using Core.Domain.Comments.Entities;
using Core.Domain.Comments.Repositories;

namespace Common.Infrastructure.Data.Repositories;

internal sealed class EfCommentRepository(AppDbContext dbContext) : ICommentRepository
{
    public Task AddAsync(Comment comment)
    {
        return dbContext.Set<Comment>().AddAsync(comment).AsTask();
    }

    public async Task<Comment> GetByIdAsync(Guid id)
    {
        return await dbContext.Set<Comment>().FindAsync(id) ??
               throw new InvalidOperationException("Comment not found");
    }
}