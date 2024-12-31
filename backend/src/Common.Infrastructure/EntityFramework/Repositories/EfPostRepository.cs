using Core.Domain.Posts.Entities;
using Core.Domain.Posts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.EntityFramework.Repositories;

internal sealed class EfPostRepository(AppDbContext dbContext) : IPostRepository
{
    public async Task<Post> GetByIdAsync(Guid id)
    {
        return await dbContext.Set<Post>().AsNoTracking().FirstAsync(p => p.Id == id);
    }

    public async Task AddAsync(Post post)
    {
        await dbContext.Set<Post>().AddAsync(post);
        await dbContext.SaveChangesAsync();
    }

    public Task UpdateAsync(Post post)
    {
        dbContext.Set<Post>().Update(post);
        return dbContext.SaveChangesAsync();
    }

    public Task RemoveAsync(Guid id)
    {
        return dbContext.Set<Post>()
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync();
    }
}