using Common.Application.Queues;
using Common.Infrastructure.RabbitMq.Configurations;
using Common.Infrastructure.RabbitMq.Queues;
using Common.Infrastructure.RabbitMq.Workers;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Common.Infrastructure.RabbitMq.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonInfrastructureRabbitMq(this IServiceCollection services, string connectionString, Action<RabbitMqConfiguration>? configure = null)
    {
        var factory = new ConnectionFactory
        {
            Uri = new(connectionString)
        };

        var configuration = new RabbitMqConfiguration();
        configure?.Invoke(configuration);

        services.AddSingleton(configuration);

        foreach (var consumer in configuration.Consumers)
        {
            services.AddTransient(consumer.Value.ConsumerType);
        }

        var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();

        services.AddHostedService<SingleConsumerWorker>();

        return services
            .AddSingleton(connection)
            .AddScoped<IAsyncQueue, RabbitMqAsyncQueue>();
    }
}