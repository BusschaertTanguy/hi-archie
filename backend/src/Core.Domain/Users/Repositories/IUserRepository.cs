using Core.Domain.Users.Entities;

namespace Core.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserAsync(string externalId);
    Task AddAsync(User user);
}