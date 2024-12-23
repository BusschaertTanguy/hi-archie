using Core.Domain.Comments.Entities;
using Core.Domain.Comments.Repositories;
using Core.Domain.Posts.Entities;
using Core.Domain.Posts.Repositories;

namespace Common.Infrastructure.Data.Repositories;

internal sealed class EfCommentRepository(AppDbContext dbContext) : ICommentRepository
{
    public Task AddAsync(Comment comment)
    {
        return dbContext.Set<Comment>().AddAsync(comment).AsTask();
    }
}