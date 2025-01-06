using Common.Application.Models;

namespace Core.Application.Comments.Events;

public class CommentEdited : IAsyncMessage
{
    public required Guid Id { get; init; }
}