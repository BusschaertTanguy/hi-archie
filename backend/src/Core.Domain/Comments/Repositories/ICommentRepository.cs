using Core.Domain.Comments.Entities;

namespace Core.Domain.Comments.Repositories;

public interface ICommentRepository
{
    Task<Comment> GetByIdAsync(Guid id);
    public Task AddAsync(Comment comment);
    Task UpdateAsync(Comment comment);
}