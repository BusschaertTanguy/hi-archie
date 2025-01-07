using Common.Application.Projections;
using Core.Application.Posts.Events;
using Neo4j.Driver;

namespace Host.QueueListener.ProjectionWriters;

internal sealed class PostRemovedConsumer(IDriver driver) : IProjectEvent<PostRemoved>
{
    public async Task HandleAsync(PostRemoved @event)
    {
        await using var session = driver.AsyncSession();

        await session.ExecuteWriteAsync(tx => tx.RunAsync(
            "MATCH (p:POST {id: $id})<-[r:IS_COMMENT_OF*..]-(c:COMMENT) " +
            "DETACH DELETE p, c;",
            new
            {
                id = @event.Id.ToString()
            }
        ));
    }
}