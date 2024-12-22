using Common.Application.Models;
using Common.Application.Queries;
using Core.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Users.Queries;

public static class GetUserByExternalId
{
    public sealed record Request(string ExternalId) : IQuery<Response?>;

    public sealed record Response(Guid Id, IEnumerable<Guid> JoinedCommunities);

    internal sealed class Handler(IQueryProcessor queryProcessor) : IQueryHandler<Request, Response?>
    {
        public async Task<Result<Response?>> HandleAsync(Request request)
        {
            var user = await queryProcessor.Query<User>()
                .Include(u => u.Subscriptions)
                .Where(u => u.ExternalId == request.ExternalId)
                .Select(u => new Response(u.Id, u.Subscriptions.Select(s => s.CommunityId)))
                .FirstOrDefaultAsync();

            return Result<Response?>.Success(user);
        }
    }
}