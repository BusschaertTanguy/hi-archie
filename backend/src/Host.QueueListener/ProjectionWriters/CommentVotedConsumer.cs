using Common.Application.Projections;
using Common.Infrastructure.RabbitMq.Models;
using Core.Application.Comments.Events;
using Neo4j.Driver;

namespace Host.QueueListener.ProjectionWriters;

internal sealed class CommentVotedConsumer(IDriver driver) : IProjectEvent<Batch<CommentVoted>>
{
    public async Task HandleAsync(Batch<CommentVoted> batch)
    {
        await using var session = driver.AsyncSession();

        foreach (var commentVote in batch.Items.GroupBy(cv => cv.Id).Select(cv => new { Id = cv.Key, Up = cv.Sum(v => v.UpChange), Down = cv.Sum(v => v.DownChange) }))
        {
            await session.ExecuteWriteAsync(tx =>
                tx.RunAsync(
                    "MATCH (c:COMMENT {id: $id}) " +
                    "SET c.up = c.up + ($up) " +
                    "SET c.down = c.down + ($down);",
                    new
                    {
                        id = commentVote.Id.ToString(),
                        up = commentVote.Up,
                        down = commentVote.Down
                    }
                ));
        }
    }
}