using Common.Infrastructure.Neo4j.Projections;
using Core.Application.Comments.Projections;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;

namespace Common.Infrastructure.Neo4J.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonInfrastructureNeo4J(this IServiceCollection services, string url, string username, string password)
    {
        return services
            .AddSingleton(GraphDatabase.Driver(url, AuthTokens.Basic(username, password)))
            .AddTransient<ICommentProjectionReader, Neo4JCommentProjectionReader>();
    }
}