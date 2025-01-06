using Common.Application.Models;

namespace Core.Application.Comments.Events;

public class CommentVoted : IAsyncMessage
{
    public required Guid Id { get; init; }
    public required int UpChange { get; init; }
    public required int DownChange { get; init; }
}