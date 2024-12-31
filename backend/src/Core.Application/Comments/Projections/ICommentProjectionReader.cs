namespace Core.Application.Comments.Projections;

public interface ICommentProjectionReader
{
    Task<List<CommentProjection>> GetCommentsAsync(Guid postId);
}