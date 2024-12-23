using Core.Domain.Posts.Entities;
using Core.Domain.Posts.Repositories;

namespace Common.Infrastructure.Data.Repositories;

internal sealed class EfPostRepository(AppDbContext dbContext) : IPostRepository
{
    public Task AddAsync(Post post)
    {
        return dbContext.Set<Post>().AddAsync(post).AsTask();
    }

    public async Task<Post> GetByIdAsync(Guid id)
    {
        return await dbContext.Set<Post>().FindAsync(id) ??
               throw new InvalidOperationException("Post not found");
    }

    public Task RemoveAsync(Post post)
    {
        dbContext.Set<Post>().Remove(post);
        return Task.CompletedTask;
    }
}