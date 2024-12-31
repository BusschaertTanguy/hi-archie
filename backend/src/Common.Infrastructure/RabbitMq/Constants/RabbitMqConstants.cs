using Core.Application.Comments.Events;
using Core.Application.Posts.Events;

namespace Common.Infrastructure.RabbitMq.Constants;

public static class RabbitMqConstants
{
    public static readonly List<string> Queues = [PostAdded.QueueName, CommentAdded.QueueName, CommentEdited.QueueName, CommentVoted.QueueName, PostRemoved.QueueName];
}