using Neo4j.Driver;

namespace Host.QueueListener.Projections;

public interface IProjectEvent<in T>
{
    Task HandleAsync(IAsyncTransaction tx, T @event);
}