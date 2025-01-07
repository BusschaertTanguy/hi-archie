using System.Reflection;
using Common.Application.Projections;
using Common.Infrastructure.Extensions;

namespace Common.Infrastructure.RabbitMq.Configurations;

public sealed class RabbitMqConfiguration
{
    private readonly Dictionary<string, RabbitMqConsumer> _consumers = new();
    public IReadOnlyDictionary<string, RabbitMqConsumer> Consumers => _consumers;

    public void AddConsumers(Assembly assembly)
    {
        var interfaceType = typeof(IProjectEvent<>);
        var consumerTypes = assembly.GetTypes().Where(t => !t.IsInterface && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType));

        foreach (var consumerType in consumerTypes)
        {
            AddConsumer(consumerType);
        }
    }

    public void AddConsumer<T>(Action<RabbitMqConsumer>? configure = null)
    {
        AddConsumer(typeof(T), configure);
    }

    private void AddConsumer(Type type, Action<RabbitMqConsumer>? configure = null)
    {
        var consumerName = type.FullName.ToKebabCase();

        if (!_consumers.TryGetValue(consumerName, out var consumer))
        {
            consumer = new()
            {
                ConsumerType = type
            };

            configure?.Invoke(consumer);
            _consumers.Add(consumerName, consumer);
        }
        else
        {
            configure?.Invoke(consumer);
        }

        var interfaceType = typeof(IProjectEvent<>);
        if (consumer.ConsumerType.GetInterfaces().Count(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType) > 1)
        {
            throw new InvalidOperationException($"{interfaceType} can only be implemented once per class");
        }
    }
}