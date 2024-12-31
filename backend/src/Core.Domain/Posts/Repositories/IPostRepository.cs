using Core.Domain.Posts.Entities;

namespace Core.Domain.Posts.Repositories;

public interface IPostRepository
{
    Task<Post> GetByIdAsync(Guid id);
    Task AddAsync(Post post);
    Task UpdateAsync(Post post);
    Task RemoveAsync(Guid id);
}