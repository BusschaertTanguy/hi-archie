namespace Common.Infrastructure.RabbitMq.Configurations;

public sealed class RabbitMqConsumerBatch
{
    public ushort Size { get; init; } = 100;
    public TimeSpan Delay { get; init; } = TimeSpan.FromSeconds(3);
}