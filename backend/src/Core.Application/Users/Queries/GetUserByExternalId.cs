using Common.Application.Models;
using Common.Application.Queries;
using Core.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Users.Queries;

public static class GetUserByExternalId
{
    public sealed record Request(string ExternalId) : IQuery<Response?>;

    public sealed record Response(Guid Id);

    internal sealed class Handler(IQueryProcessor queryProcessor) : IQueryHandler<Request, Response?>
    {
        public async Task<Result<Response?>> HandleAsync(Request query)
        {
            var user = await queryProcessor.Query<User>()
                .Where(u => u.ExternalId == query.ExternalId)
                .Select(u => new Response(u.Id))
                .FirstOrDefaultAsync();

            return Result<Response?>.Success(user);
        }
    }
}