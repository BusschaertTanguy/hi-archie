namespace Common.Infrastructure.RabbitMq.Configurations;

public sealed class RabbitMqConsumer
{
    public required Type ConsumerType { get; init; }
}