using System.Text.Json;
using Common.Application.Projections;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.RabbitMq.Configurations;
using Common.Infrastructure.RabbitMq.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.Infrastructure.RabbitMq.Workers;

internal sealed class SingleConsumerWorker(ILogger<SingleConsumerWorker> logger, IConnection connection, IServiceScopeFactory serviceScopeFactory, RabbitMqConfiguration configuration)
    : BackgroundService
{
    private IChannel? _channel;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await _channel.BasicQosAsync(0, 1, false, cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null)
        {
            return;
        }

        var interfaceType = typeof(IProjectEvent<>);

        foreach (var (key, consumer) in configuration.Consumers.Where(c => c.Value.Batch == null))
        {
            var queue = await _channel.QueueDeclareAsync(key, true, false, false, cancellationToken: stoppingToken);

            var messageInterface = consumer.ConsumerType.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
            var messageInterfaceType = messageInterface.GenericTypeArguments.Single();
            var messageName = messageInterfaceType.FullName?.ToKebabCase();

            if (string.IsNullOrWhiteSpace(messageName))
            {
                continue;
            }

            await _channel.ExchangeDeclareAsync(messageName, ExchangeType.Fanout, true, cancellationToken: stoppingToken);

            await _channel.QueueBindAsync(
                queue.QueueName,
                messageName,
                string.Empty,
                cancellationToken: stoppingToken
            );

            var eventConsumer = new AsyncEventingBasicConsumer(_channel);

            eventConsumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var queueName = ea.ConsumerTag;

                    if (!configuration.Consumers.TryGetValue(queueName, out var handlerType))
                    {
                        throw new InvalidOperationException($"Could not find handler for {queueName}");
                    }

                    await using var scope = serviceScopeFactory.CreateAsyncScope();
                    var handler = scope.ServiceProvider.GetRequiredService(handlerType.ConsumerType);

                    var body = ea.Body.ToArray();
                    var message = JsonSerializer.Deserialize<EventData>(body) ?? throw new InvalidOperationException("Could not deserialize body");
                    var messageType = Type.GetType(message.Type) ?? throw new InvalidOperationException("Could not deserialize type");
                    var content = JsonSerializer.Deserialize(message.Content, messageType);

                    var method = handlerType.ConsumerType.GetMethod(nameof(IProjectEvent<object>.HandleAsync), [messageType]) ??
                                 throw new InvalidOperationException("Could not find handler method");

                    if (method.Invoke(handler, [content]) is Task task)
                    {
                        await task;
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                }
                catch (Exception e)
                {
                    logger.LogError(e, e.Message);
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
                }
            };

            await _channel.BasicConsumeAsync(queue, false, queue.QueueName, eventConsumer, stoppingToken);
        }
    }
}