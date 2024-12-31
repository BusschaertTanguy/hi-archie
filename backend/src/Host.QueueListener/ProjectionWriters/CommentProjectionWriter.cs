using Core.Application.Comments.Events;
using Core.Application.Posts.Events;
using Core.Domain.Comments.Repositories;
using Host.QueueListener.Projections;
using Neo4j.Driver;

namespace Host.QueueListener.ProjectionWriters;

internal sealed class CommentProjectionWriter(ICommentRepository commentRepository) :
    IProjectEvent<PostAdded>,
    IProjectEvent<CommentAdded>,
    IProjectEvent<CommentEdited>,
    IProjectEvent<CommentVoted>,
    IProjectEvent<PostRemoved>
{
    public async Task HandleAsync(IAsyncTransaction tx, CommentAdded @event)
    {
        var comment = await commentRepository.GetByIdAsync(@event.Id);

        if (comment.ParentId.HasValue)
        {
            await tx.RunAsync(
                "MATCH (p:COMMENT { id: $parentId }) " +
                "MERGE (c:COMMENT {id: $commentId, content: $content, publishDate: $publishDate, ownerId: $ownerId, up: 0, down: 0}) " +
                "MERGE (c)-[:IS_COMMENT_OF]->(p)",
                new
                {
                    parentId = comment.ParentId.Value.ToString(),
                    commentId = comment.Id.ToString(),
                    content = comment.Content,
                    publishDate = comment.PublishDate,
                    ownerId = comment.OwnerId.ToString()
                }
            );
        }
        else
        {
            await tx.RunAsync(
                "MATCH (p:POST { id: $postId }) " +
                "MERGE (c:COMMENT {id: $commentId, content: $content, publishDate: $publishDate, ownerId: $ownerId, up: 0, down: 0}) " +
                "MERGE (c)-[:IS_COMMENT_OF]->(p)",
                new
                {
                    postId = comment.PostId.ToString(),
                    commentId = comment.Id.ToString(),
                    content = comment.Content,
                    publishDate = comment.PublishDate,
                    ownerId = comment.OwnerId.ToString()
                }
            );
        }
    }

    public async Task HandleAsync(IAsyncTransaction tx, CommentEdited @event)
    {
        var comment = await commentRepository.GetByIdAsync(@event.Id);

        await tx.RunAsync(
            "MATCH (c:COMMENT {id: $id}) " +
            "SET c.content = $content",
            new
            {
                id = comment.Id.ToString(),
                content = comment.Content
            }
        );
    }

    public async Task HandleAsync(IAsyncTransaction tx, CommentVoted @event)
    {
        await tx.RunAsync(
            "MATCH (c:COMMENT {id: $id}) " +
            "SET c.up = c.up + ($up) " +
            "SET c.down = c.down + ($down);",
            new
            {
                id = @event.Id.ToString(),
                up = @event.UpChange,
                down = @event.DownChange
            }
        );
    }

    public async Task HandleAsync(IAsyncTransaction tx, PostAdded @event)
    {
        await tx.RunAsync("MERGE (:POST { id: $id });", new { id = @event.Id.ToString() });
    }

    public async Task HandleAsync(IAsyncTransaction tx, PostRemoved @event)
    {
        await tx.RunAsync(
            "MATCH (p:POST {id: $id})<-[r:IS_COMMENT_OF*..]-(c:COMMENT) " +
            "DETACH DELETE p, c;",
            new
            {
                id = @event.Id.ToString()
            });
    }
}