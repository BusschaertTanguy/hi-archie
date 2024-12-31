using Core.Domain.Comments.Entities;
using Core.Domain.Comments.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.EntityFramework.Repositories;

internal sealed class EfCommentRepository(AppDbContext dbContext) : ICommentRepository
{
    public Task<Comment> GetByIdAsync(Guid id)
    {
        return dbContext.Set<Comment>().AsNoTracking().FirstAsync(c => c.Id == id);
    }

    public async Task AddAsync(Comment comment)
    {
        await dbContext.Set<Comment>().AddAsync(comment);
        await dbContext.SaveChangesAsync();
    }

    public Task UpdateAsync(Comment comment)
    {
        dbContext.Set<Comment>().Update(comment);
        return dbContext.SaveChangesAsync();
    }
}