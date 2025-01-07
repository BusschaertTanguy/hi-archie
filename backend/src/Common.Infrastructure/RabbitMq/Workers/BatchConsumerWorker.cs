using System.Reflection;
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

internal sealed class BatchConsumerWorker(ILogger<BatchConsumerWorker> logger, IConnection connection, IServiceScopeFactory serviceScopeFactory, RabbitMqConfiguration configuration)
    : BackgroundService
{
    private readonly Dictionary<string, IChannel> _channels = new();
    private readonly Dictionary<string, List<Entry>> _queueBatches = new();
    private readonly Dictionary<string, CancellationTokenSource> _queueTimerCancellations = new();

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var channel in _channels.Values)
        {
            await channel.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        foreach (var channel in _channels.Values)
        {
            channel.Dispose();
        }

        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interfaceType = typeof(IProjectEvent<>);

        foreach (var (key, consumer) in configuration.Consumers.Where(c => c.Value.Batch != null))
        {
            if (consumer.Batch == null)
            {
                continue;
            }

            var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
            await channel.BasicQosAsync(0, consumer.Batch.Size, false, stoppingToken);

            _channels[key] = channel;

            var queue = await channel.QueueDeclareAsync(key, true, false, false, cancellationToken: stoppingToken);
            _queueBatches[key] = [];

            var messageInterface = consumer.ConsumerType.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
            var batchInterfaceType = messageInterface.GenericTypeArguments.Single();
            var messageInterfaceType = batchInterfaceType.GenericTypeArguments.Single();
            var messageName = messageInterfaceType.FullName?.ToKebabCase();

            if (string.IsNullOrWhiteSpace(messageName))
            {
                continue;
            }

            await channel.ExchangeDeclareAsync(messageName, ExchangeType.Fanout, true, cancellationToken: stoppingToken);

            await channel.QueueBindAsync(
                queue.QueueName,
                messageName,
                string.Empty,
                cancellationToken: stoppingToken
            );

            var eventConsumer = new AsyncEventingBasicConsumer(channel);

            eventConsumer.ReceivedAsync += async (_, ea) =>
            {
                var queueName = ea.ConsumerTag;
                var batch = _queueBatches[queueName];

                try
                {
                    var body = ea.Body.ToArray();
                    var message = JsonSerializer.Deserialize<EventData>(body) ?? throw new InvalidOperationException("Could not deserialize body");
                    var messageType = Type.GetType(message.Type) ?? throw new InvalidOperationException("Could not deserialize type");

                    batch.Add(new(ea.DeliveryTag, messageType, message.Content));
                }
                catch (Exception e)
                {
                    logger.LogError(e, e.Message);
                    await channel.BasicNackAsync(ea.DeliveryTag, true, true, stoppingToken);
                    batch.Clear();
                }

                if (batch.Count > 0 && !_queueTimerCancellations.ContainsKey(queueName))
                {
                    var cancellationToken = new CancellationTokenSource();
                    _queueTimerCancellations[queueName] = cancellationToken;

                    Task.Delay(consumer.Batch.Delay, cancellationToken.Token).ContinueWith(_ =>
                    {
                        logger.LogInformation("Delayed processing of batch: {queueName}", queueName);
                        return ProcessBatch(queueName);
                    }, cancellationToken.Token);
                }

                if (batch.Count < consumer.Batch.Size)
                {
                    return;
                }

                var token = _queueTimerCancellations[queueName];
                await token.CancelAsync();

                await ProcessBatch(queueName);
            };

            await channel.BasicConsumeAsync(queue, false, queue.QueueName, eventConsumer, stoppingToken);
        }
    }

    private async Task ProcessBatch(string queueName)
    {
        logger.LogInformation("Processing of batch for {queueName} started", queueName);

        var channel = _channels[queueName];
        var batch = _queueBatches[queueName];
        var lastEntry = batch.LastOrDefault();

        if (lastEntry == null)
        {
            return;
        }

        try
        {
            if (!configuration.Consumers.TryGetValue(queueName, out var handlerType))
            {
                throw new InvalidOperationException($"Could not find handler for {queueName}");
            }

            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService(handlerType.ConsumerType);

            var genericBatchType = typeof(Batch<>);
            var messageType = batch.Select(e => e.Type).Distinct().Single();
            var batchType = genericBatchType.MakeGenericType(messageType);

            var batchedItems = Activator.CreateInstance(batchType);
            var batchItemsAdder = batchType.GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("No add method on batch");

            foreach (var item in batch.Select(entry => JsonSerializer.Deserialize(entry.Content, entry.Type)))
            {
                batchItemsAdder?.Invoke(batchedItems, [item]);
            }

            var method = handlerType.ConsumerType.GetMethod(nameof(IProjectEvent<object>.HandleAsync), [batchType]) ?? throw new InvalidOperationException("Could not find handler method");

            if (method.Invoke(handler, [batchedItems]) is Task task)
            {
                await task;
            }

            await channel.BasicAckAsync(lastEntry.DeliveryTag, true);
            batch.Clear();
        }
        catch (Exception e)
        {
            logger.LogError("Error while processing of batch for {queueName}", queueName);
            logger.LogDebug(e, e.Message);

            await channel.BasicNackAsync(lastEntry.DeliveryTag, true, true);
            batch.Clear();
        }
        finally
        {
            _queueTimerCancellations.Remove(queueName);
            logger.LogInformation("Processing of batch for {queueName} successful", queueName);
        }
    }

    private sealed record Entry(ulong DeliveryTag, Type Type, byte[] Content);
}