namespace Core.Application.Posts.Events;

public sealed class PostAdded
{
    public const string QueueName = "post-added";
    public required Guid Id { get; init; }
}