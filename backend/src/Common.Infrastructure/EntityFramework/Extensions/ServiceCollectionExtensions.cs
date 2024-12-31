using Common.Application.Commands;
using Common.Application.Queries;
using Common.Infrastructure.EntityFramework.Commands;
using Common.Infrastructure.EntityFramework.Queries;
using Common.Infrastructure.EntityFramework.Repositories;
using Core.Domain.Comments.Repositories;
using Core.Domain.Communities.Repositories;
using Core.Domain.Posts.Repositories;
using Core.Domain.Users.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.EntityFramework.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonInfrastructureEntityFramework(this IServiceCollection services,
        string connectionString)
    {
        return services
            .AddDbContext<AppDbContext>(builder => builder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention())
            .AddTransient<IUnitOfWork, EfUnitOfWork>()
            .AddTransient<IQueryProcessor, EfQueryProcessor>()
            .AddTransient<ICommunityRepository, EfCommunityRepository>()
            .AddTransient<IPostRepository, EfPostRepository>()
            .AddTransient<IPostVoteRepository, EfPostVoteRepository>()
            .AddTransient<ICommentRepository, EfCommentRepository>()
            .AddTransient<ICommentVoteRepository, EfCommentVoteRepository>()
            .AddTransient<ISubscriptionRepository, EfSubscriptionRepository>()
            .AddTransient<IUserRepository, EfUserRepository>();
    }
}