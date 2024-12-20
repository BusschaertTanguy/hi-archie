using Core.Domain.Posts.Entities;
using Core.Domain.Posts.Repositories;

namespace Common.Infrastructure.Data.Repositories;

internal sealed class EfPostRepository(AppDbContext dbContext) : IPostRepository
{
    public Task AddAsync(Post post)
    {
        return dbContext.Set<Post>().AddAsync(post).AsTask();
    }
}