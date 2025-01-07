using Common.Application.Projections;
using Core.Application.Comments.Events;
using Neo4j.Driver;

namespace Host.QueueListener.ProjectionWriters;

internal sealed class CommentVotedConsumer(IDriver driver) : IProjectEvent<CommentVoted>
{
    public async Task HandleAsync(CommentVoted @event)
    {
        await using var session = driver.AsyncSession();

        await session.ExecuteWriteAsync(tx =>
            tx.RunAsync(
                "MATCH (c:COMMENT {id: $id}) " +
                "SET c.up = c.up + ($up) " +
                "SET c.down = c.down + ($down);",
                new
                {
                    id = @event.Id.ToString(),
                    up = @event.UpChange,
                    down = @event.DownChange
                }
            ));
    }
}