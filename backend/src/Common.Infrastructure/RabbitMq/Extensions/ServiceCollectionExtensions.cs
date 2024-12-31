using Common.Application.Queues;
using Common.Infrastructure.RabbitMq.Constants;
using Common.Infrastructure.RabbitMq.Queues;
using Core.Application.Comments.Events;
using Core.Application.Posts.Events;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Common.Infrastructure.RabbitMq.Extensions;

public static class ServiceCollectionExtensions
{
    public static async Task<IServiceCollection> AddCommonInfrastructureRabbitMq(this IServiceCollection services, string connectionString)
    {
        var factory = new ConnectionFactory
        {
            Uri = new(connectionString)
        };

        var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        foreach (var queue in RabbitMqConstants.Queues)
        {
            await channel.QueueDeclareAsync(queue, false, false, false);
        }

        return services
            .AddSingleton(connection)
            .AddScoped<IAsyncQueue, RabbitMqAsyncQueue>();
    }
}