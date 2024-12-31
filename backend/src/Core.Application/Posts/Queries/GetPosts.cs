using Common.Application.Models;
using Common.Application.Queries;
using Core.Domain.Posts.Entities;
using Core.Domain.Posts.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Posts.Queries;

public static class GetPosts
{
    public sealed record Request(Guid CommunityId, int PageIndex, int PageSize, string? Title, Guid? UserId) : IQuery<Response>;

    public sealed class Dto
    {
        public required Guid Id { get; init; }
        public required string Title { get; init; }
        public required DateTime PublishDate { get; init; }
        public required long Up { get; init; }
        public required long Down { get; init; }
        public PostVoteType? CurrentVote { get; init; }
    }

    public sealed class Response
    {
        public required List<Dto> Posts { get; init; }
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

            var (communityId, pageIndex, pageSize, title, userId) = request;

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
                .Include(p => p.Votes)
                .OrderByDescending(p => p.Votes.Count(v => v.Type == PostVoteType.Upvote) - p.Votes.Count(v => v.Type == PostVoteType.Downvote))
                .ThenByDescending(p => p.PublishDate)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(p => new Dto
                {
                    Id = p.Id,
                    Title = p.Title,
                    PublishDate = p.PublishDate,
                    Up = p.Votes.Count(v => v.Type == PostVoteType.Upvote),
                    Down = p.Votes.Count(v => v.Type == PostVoteType.Downvote),
                    CurrentVote = p.Votes.FirstOrDefault(v => v.UserId == userId)!.Type
                })
                .ToListAsync();

            return Result<Response>.Success(new() { Posts = posts, Total = total });
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