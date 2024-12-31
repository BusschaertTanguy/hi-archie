using System.Text.Json;
using Common.Application.Queues;
using Common.Infrastructure.RabbitMq.Models;
using RabbitMQ.Client;

namespace Common.Infrastructure.RabbitMq.Queues;

internal sealed class RabbitMqAsyncQueue(IConnection connection) : IAsyncQueue
{
    private IChannel? _channel;

    public async Task PublishAsync<T>(string queue, T message)
    {
        _channel ??= await connection.CreateChannelAsync();

        var type = typeof(T).AssemblyQualifiedName ?? throw new InvalidOperationException("AssemblyQualifiedName is null");
        var content = JsonSerializer.SerializeToUtf8Bytes(message);
        var eventData = new EventData(type, content);
        var body = JsonSerializer.SerializeToUtf8Bytes(eventData);

        await _channel.BasicPublishAsync(string.Empty, queue, body);
    }
}