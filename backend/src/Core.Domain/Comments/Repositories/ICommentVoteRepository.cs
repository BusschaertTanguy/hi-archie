using Core.Domain.Comments.Entities;
using Core.Domain.Posts.Entities;

namespace Core.Domain.Comments.Repositories;

public interface ICommentVoteRepository
{
    Task<CommentVote?> GetByIdAsync(Guid commentId, Guid userId);
    Task AddAsync(CommentVote vote);
    Task RemoveAsync(CommentVote vote);
}