using Common.Application.Models;
using Common.Application.Queries;
using Core.Domain.Posts.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Posts.Queries;

public static class GetPosts
{
    public sealed record Request(Guid CommunityId, int PageIndex, int PageSize, string? Title) : IQuery<Response>;

    public sealed record Dto(Guid Id, string Title, DateTime PublishDate);

    public sealed record Response(List<Dto> Posts, long Total);

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

            var (communityId, pageIndex, pageSize, title) = request;

            var postsQuery = queryProcessor
                .Query<Post>()
                .Where(p => p.CommunityId == communityId);

            if (!string.IsNullOrWhiteSpace(title))
            {
                postsQuery = postsQuery.Where(p => p.Title.Contains(title));
            }

            var total = await postsQuery
                .CountAsync();

            var posts = await postsQuery
                .OrderByDescending(p => p.PublishDate)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(p => new Dto(p.Id, p.Title, p.PublishDate))
                .ToListAsync();

            return Result<Response>.Success(new(posts, total));
        }
    }

    private sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.CommunityId)
                .NotEmpty();

            RuleFor(r => r.PageIndex)
                .GreaterThanOrEqualTo(0);

            RuleFor(r => r.PageSize)
                .Equal(25);
        }
    }
}