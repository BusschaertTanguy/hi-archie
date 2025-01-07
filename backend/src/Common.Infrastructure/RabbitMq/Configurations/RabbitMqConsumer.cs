namespace Common.Infrastructure.RabbitMq.Configurations;

public sealed class RabbitMqConsumer
{
    public RabbitMqConsumerBatch? Batch { get; set; }
    public required Type ConsumerType { get; init; }
}