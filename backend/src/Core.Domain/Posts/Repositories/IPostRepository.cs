using Core.Domain.Posts.Entities;

namespace Core.Domain.Posts.Repositories;

public interface IPostRepository
{
    Task AddAsync(Post post);
    Task<Post> GetByIdAsync(Guid id);
    Task RemoveAsync(Post post);
}