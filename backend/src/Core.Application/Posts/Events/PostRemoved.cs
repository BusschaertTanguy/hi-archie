using Common.Application.Models;

namespace Core.Application.Posts.Events;

public sealed class PostRemoved : IAsyncMessage
{
    public required Guid Id { get; init; }
}