using Core.Domain.Comments.Enums;

namespace Core.Domain.Comments.Entities;

public sealed class CommentVote
{
    public required Guid CommentId { get; init; }
    public required Guid UserId { get; init; }
    public required CommentVoteType Type { get; set; }
}