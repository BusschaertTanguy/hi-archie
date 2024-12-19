using Core.Domain.Users.Entities;

namespace Core.Domain.Communities.Entities;

public sealed class Community
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public required Guid OwnerId { get; init; }

    public User? Owner { get; } = null;
}