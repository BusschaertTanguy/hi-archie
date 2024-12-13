using Core.Domain.Users.Entities;
using Core.Domain.Users.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.Data.Repositories;

internal sealed class EfUserRepository(AppDbContext dbContext) : IUserRepository
{
    public Task<User?> GetUserAsync(string externalId)
    {
        return dbContext.Set<User>().FirstOrDefaultAsync(u => u.ExternalId == externalId);
    }

    public Task AddAsync(User user)
    {
        return dbContext.Set<User>().AddAsync(user).AsTask();
    }
}