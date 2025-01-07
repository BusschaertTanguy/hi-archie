using Common.Application.Projections;
using Core.Application.Comments.Events;
using Core.Domain.Comments.Repositories;
using Neo4j.Driver;

namespace Host.QueueListener.ProjectionWriters;

internal sealed class CommentAddedConsumer(ICommentRepository commentRepository, IDriver driver) : IProjectEvent<CommentAdded>
{
    public async Task HandleAsync(CommentAdded @event)
    {
        await using var session = driver.AsyncSession();
        var comment = await commentRepository.GetByIdAsync(@event.Id);

        if (comment.ParentId.HasValue)
        {
            await session.ExecuteWriteAsync(tx =>
                tx.RunAsync(
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
                ));
        }
        else
        {
            await session.ExecuteWriteAsync(tx =>
                tx.RunAsync(
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
                ));
        }
    }
}