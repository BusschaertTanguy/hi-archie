namespace Core.Domain.Users.Entities;

public sealed class User
{
    public required Guid Id { get; set; }
    public required string ExternalId { get; set; }
}