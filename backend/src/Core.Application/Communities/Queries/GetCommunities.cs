using Common.Application.Models;
using Common.Application.Queries;
using Core.Domain.Communities.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Communities.Queries;

public static class GetCommunities
{
    public sealed record Request(int PageIndex, int PageSize, string? Name) : IQuery<Response>;

    public sealed class Dto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required Guid OwnerId { get; init; }
    }

    public sealed class Response
    {
        public required List<Dto> Communities { get; init; }
        public required long Total { get; init; }
    }

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

            var (pageIndex, pageSize, name) = request;

            var communitiesQuery = queryProcessor
                .Query<Community>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                communitiesQuery = communitiesQuery.Where(c => c.Name.Contains(name));
            }

            var total = await communitiesQuery
                .CountAsync();

            var communities = await communitiesQuery
                .OrderBy(c => c.Name)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(c => new Dto { Id = c.Id, Name = c.Name, OwnerId = c.OwnerId })
                .ToListAsync();

            return Result<Response>.Success(new() { Communities = communities, Total = total });
        }
    }

    private sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.PageIndex)
                .GreaterThanOrEqualTo(0);

            RuleFor(r => r.PageSize)
                .Equal(25);
        }
    }
}