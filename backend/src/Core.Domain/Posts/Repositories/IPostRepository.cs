using Core.Domain.Posts.Entities;

namespace Core.Domain.Posts.Repositories;

public interface IPostRepository
{
    Task AddAsync(Post post);
}