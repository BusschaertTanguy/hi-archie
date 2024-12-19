using Common.Application.Models;
using Common.Application.Queries;
using Core.Domain.Communities.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Communities.Queries;

public static class GetCommunities
{
    public sealed record Request(int PageIndex, int PageSize, string? Name) : IQuery<Response>;

    public sealed record Dto(Guid Id, string Name, Guid OwnerId);

    public sealed record Response(List<Dto> Communities, long Total);

    internal sealed class Handler(IValidator<Request> validator, IQueryProcessor queryProcessor)
        : IQueryHandler<Request, Response>
    {
        public async Task<Result<Response>> HandleAsync(Request request)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Result<Response>.Failure("validation-failed");
            }

            var communitiesQuery = queryProcessor
                .Query<Community>();

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                communitiesQuery = communitiesQuery.Where(c => c.Name.Contains(request.Name));
            }

            var total = await communitiesQuery
                .CountAsync();

            var communities = await communitiesQuery
                .OrderBy(c => c.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Dto(c.Id, c.Name, c.OwnerId))
                .ToListAsync();

            return Result<Response>.Success(new(communities, total));
        }
    }

    private sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(q => q.PageIndex)
                .GreaterThanOrEqualTo(0);

            RuleFor(q => q.PageSize)
                .Equal(25);
        }
    }
}