namespace Common.Application.Queues;

public interface IAsyncQueue
{
    Task PublishAsync<T>(string queue, T message);
}