using Common.Application.Models;
using Common.Application.Queries;
using Core.Domain.Posts.Entities;
using Core.Domain.Posts.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Posts.Queries;

public static class GetPost
{
    public sealed record Request(Guid Id, Guid? UserId) : IQuery<Response>;

    public sealed record Response
    {
        public required Guid Id { get; init; }
        public required string Title { get; init; }
        public required string Content { get; init; }
        public required DateTime PublishDate { get; init; }
        public required Guid OwnerId { get; init; }
        public required long Up { get; init; }
        public required long Down { get; init; }
        public PostVoteType? CurrentVote { get; init; }
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

            var post = await queryProcessor.Query<Post>()
                .Include(p => p.Votes)
                .Where(p => p.Id == request.Id)
                .Select(p => new Response
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    PublishDate = p.PublishDate,
                    OwnerId = p.OwnerId,
                    Up = p.Votes.Where(v => v.Type == PostVoteType.Upvote).LongCount(),
                    Down = p.Votes.Where(v => v.Type == PostVoteType.Downvote).LongCount(),
                    CurrentVote = p.Votes.FirstOrDefault(v => v.UserId == request.UserId)!.Type
                })
                .FirstAsync();

            return Result<Response>.Success(post);
        }
    }

    private sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Id)
                .NotEmpty();
        }
    }
}