namespace Common.Infrastructure.RabbitMq.Models;

public sealed class EventData(string type, byte[] content)
{
    public string Type { get; } = type;
    public byte[] Content { get; } = content;
}