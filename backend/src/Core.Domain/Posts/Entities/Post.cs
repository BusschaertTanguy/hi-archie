﻿namespace Core.Domain.Posts.Entities;

public sealed class Post
{
    public required Guid Id { get; init; }
    public required Guid CommunityId { get; init; }
    public required string Title { get; init; }
    public required string Content { get; init; }
    public required DateTime PublishDate { get; init; }
    public required Guid OwnerId { get; init; }
}