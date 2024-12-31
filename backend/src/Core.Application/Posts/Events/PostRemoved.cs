namespace Core.Application.Posts.Events;

public sealed class PostRemoved
{
    public const string QueueName = "post-removed";
    public required Guid Id { get; init; }
}