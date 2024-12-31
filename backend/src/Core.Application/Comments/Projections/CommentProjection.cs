namespace Core.Application.Comments.Projections;

public class CommentProjection
{
    public required Guid Id { get; init; }
    public required Guid ParentId { get; init; }
    public required Guid OwnerId { get; init; }
    public required long Up { get; init; }
    public required long Down { get; init; }
    public required string Content { get; init; }
    public required DateTimeOffset PublishDate { get; init; }
    public required List<CommentProjection> Comments { get; init; }
}