namespace Core.Application.Comments.Events;

public sealed class CommentAdded
{
    public const string QueueName = "comment-added";
    public required Guid Id { get; init; }
}