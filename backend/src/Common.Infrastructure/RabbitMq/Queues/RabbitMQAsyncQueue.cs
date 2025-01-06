using System.Text.Json;
using Common.Application.Queues;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.RabbitMq.Models;
using RabbitMQ.Client;

namespace Common.Infrastructure.RabbitMq.Queues;

internal sealed class RabbitMqAsyncQueue(IConnection connection) : IAsyncQueue
{
    private IChannel? _channel;

    public async Task PublishAsync<T>(T message)
    {
        _channel ??= await connection.CreateChannelAsync();

        var typeName = typeof(T).AssemblyQualifiedName ?? throw new InvalidOperationException("AssemblyQualifiedName is null");
        var content = JsonSerializer.SerializeToUtf8Bytes(message);
        var eventData = new EventData(typeName, content);
        var body = JsonSerializer.SerializeToUtf8Bytes(eventData);

        var properties = new BasicProperties
        {
            Persistent = true
        };

        var exchange = typeof(T).FullName.ToKebabCase();
        await _channel.ExchangeDeclareAsync(exchange, ExchangeType.Fanout);
        await _channel.BasicPublishAsync(exchange, string.Empty, true, properties, body);
    }
}