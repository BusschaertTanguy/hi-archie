namespace Core.Application.Comments.Events;

public class CommentEdited
{
    public const string QueueName = "comment-edited";
    public required Guid Id { get; init; }
}