using Core.Domain.Communities.Entities;

namespace Core.Domain.Users.Entities;

public sealed class User
{
    public required Guid Id { get; set; }
    public required string ExternalId { get; set; }
    
    public List<Subscription> Subscriptions { get; } = [];
}