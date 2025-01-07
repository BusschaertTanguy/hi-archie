using Common.Application.Projections;
using Core.Application.Comments.Events;
using Core.Domain.Comments.Repositories;
using Neo4j.Driver;

namespace Host.QueueListener.ProjectionWriters;

internal sealed class CommentEditedConsumer(ICommentRepository commentRepository, IDriver driver) : IProjectEvent<CommentEdited>
{
    public async Task HandleAsync(CommentEdited @event)
    {
        await using var session = driver.AsyncSession();
        var comment = await commentRepository.GetByIdAsync(@event.Id);

        await session.ExecuteWriteAsync(tx =>
            tx.RunAsync(
                "MATCH (c:COMMENT {id: $id}) " +
                "SET c.content = $content",
                new
                {
                    id = comment.Id.ToString(),
                    content = comment.Content
                }
            ));
    }
}