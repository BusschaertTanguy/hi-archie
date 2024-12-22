namespace Core.Domain.Communities.Entities;

public sealed class Subscription
{
    public required Guid CommunityId { get; init; }
    public required Guid UserId { get; init; }
}