using Core.Domain.Comments.Entities;

namespace Core.Domain.Comments.Repositories;

public interface ICommentRepository
{
    public Task AddAsync(Comment comment);
    Task<Comment> GetByIdAsync(Guid id);
}