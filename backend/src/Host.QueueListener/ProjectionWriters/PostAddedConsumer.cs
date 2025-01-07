using Common.Application.Projections;
using Core.Application.Posts.Events;
using Neo4j.Driver;

namespace Host.QueueListener.ProjectionWriters;

internal sealed class PostAddedConsumer(IDriver driver) : IProjectEvent<PostAdded>
{
    public async Task HandleAsync(PostAdded @event)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(tx => tx.RunAsync("MERGE (:POST { id: $id });", new { id = @event.Id.ToString() }));
    }
}