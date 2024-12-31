using System.Reflection;
using System.Text.Json;
using Common.Infrastructure.RabbitMq.Constants;
using Common.Infrastructure.RabbitMq.Models;
using Host.QueueListener.Projections;
using Neo4j.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Host.QueueListener;

internal sealed class QueueListener(ILogger<QueueListener> logger, IConnection connection, IServiceScopeFactory serviceScopeFactory, IDriver driver) : BackgroundService
{
    private IChannel? _channel;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
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

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            await using var session = driver.AsyncSession();
            await using var transaction = await session.BeginTransactionAsync();

            try
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<EventData>(body) ?? throw new InvalidOperationException("Could not deserialize body");
                var type = Type.GetType(message.Type) ?? throw new InvalidOperationException("Could not deserialize type");
                var content = JsonSerializer.Deserialize(message.Content, type);

                var interfaceType = typeof(IProjectEvent<>);
                var genericType = interfaceType.MakeGenericType(type);
                var handlerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => genericType.IsAssignableFrom(t) && !t.IsInterface).ToList();

                await using var scope = serviceScopeFactory.CreateAsyncScope();

                foreach (var handlerType in handlerTypes)
                {
                    var handlerMethod = handlerType.GetMethod(nameof(IProjectEvent<object>.HandleAsync), [typeof(IAsyncTransaction), type]) ??
                                        throw new InvalidOperationException("Could not find handler method");

                    var handler = scope.ServiceProvider.GetRequiredService(handlerType);

                    if (handlerMethod.Invoke(handler, [transaction, content]) is Task task)
                    {
                        await task;
                    }
                }

                await transaction.CommitAsync();
                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                await transaction.RollbackAsync();
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
            }
        };

        foreach (var queue in RabbitMqConstants.Queues)
        {
            await _channel.BasicConsumeAsync(queue, false, consumer, stoppingToken);
        }
    }
}