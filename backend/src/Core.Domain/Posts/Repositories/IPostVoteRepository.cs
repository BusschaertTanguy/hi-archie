using Core.Domain.Posts.Entities;

namespace Core.Domain.Posts.Repositories;

public interface IPostVoteRepository
{
    Task<PostVote?> GetByIdAsync(Guid postId, Guid userId);
    Task AddAsync(PostVote vote);
    Task UpdateAsync(PostVote vote);
    Task RemoveAsync(Guid postId, Guid userId);
}