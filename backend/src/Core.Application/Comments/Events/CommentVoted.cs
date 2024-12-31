﻿namespace Core.Application.Comments.Events;

public class CommentVoted
{
    public const string QueueName = "comment-voted";
    public required Guid Id { get; init; }
    public required int UpChange { get; init; }
    public required int DownChange { get; init; }
}