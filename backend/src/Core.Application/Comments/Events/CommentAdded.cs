using Common.Application.Models;

namespace Core.Application.Comments.Events;

public sealed class CommentAdded : IAsyncMessage
{
    public required Guid Id { get; init; }
}