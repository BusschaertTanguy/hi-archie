namespace Common.Application.Queues;

public interface IAsyncQueue
{
    Task PublishAsync<T>(T message);
}