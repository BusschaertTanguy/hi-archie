namespace Core.Domain.Comments.Entities;

public sealed class Comment
{
    public required Guid Id { get; init; }
    public required string Content { get; set; }
    public required DateTime PublishDate { get; init; }
    public required Guid PostId { get; init; }
    public required Guid? ParentId { get; init; }
    public required Guid OwnerId { get; init; }
}