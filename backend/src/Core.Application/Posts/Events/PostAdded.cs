using Common.Application.Models;

namespace Core.Application.Posts.Events;

public sealed class PostAdded : IAsyncMessage
{
    public required Guid Id { get; init; }
}