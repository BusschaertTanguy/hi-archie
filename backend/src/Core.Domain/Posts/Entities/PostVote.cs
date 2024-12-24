using Core.Domain.Posts.Enums;

namespace Core.Domain.Posts.Entities;

public sealed class PostVote
{
    public required Guid PostId { get; init; }
    public required Guid UserId { get; init; }
    public required PostVoteType Type { get; set; }
}