using Common.Application.Commands;
using Common.Application.Queries;
using Common.Infrastructure.Data.Commands;
using Common.Infrastructure.Data.Queries;
using Common.Infrastructure.Data.Repositories;
using Core.Domain.Communities.Repositories;
using Core.Domain.Users.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonInfrastructureData(this IServiceCollection services, string connectionString)
    {
        return services
            .AddDbContext<AppDbContext>(builder => builder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention())
            .AddTransient<IUnitOfWork, EfUnitOfWork>()
            .AddTransient<IQueryProcessor, EfQueryProcessor>()
            .AddTransient<ICommunityRepository, EfCommunityRepository>()
            .AddTransient<IUserRepository, EfUserRepository>();
    }
}