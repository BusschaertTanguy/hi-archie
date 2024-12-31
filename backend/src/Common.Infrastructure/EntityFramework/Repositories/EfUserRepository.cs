using Core.Domain.Users.Entities;
using Core.Domain.Users.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.EntityFramework.Repositories;

internal sealed class EfUserRepository(AppDbContext dbContext) : IUserRepository
{
    public Task<User?> GetUserAsync(string externalId)
    {
        return dbContext.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.ExternalId == externalId);
    }

    public async Task AddAsync(User user)
    {
        await dbContext.Set<User>().AddAsync(user);
        await dbContext.SaveChangesAsync();
    }
}