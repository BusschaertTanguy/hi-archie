using Common.Application.Models;
using Common.Application.Queries;
using Core.Domain.Comments.Entities;
using Core.Domain.Comments.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Comments.Queries;

public static class GetComments
{
    public sealed record Request(Guid PostId) : IQuery<List<Response>>;

    public sealed record Response(Guid Id, string Content, DateTime PublishDate, Guid OwnerId, Guid? ParentId, long Up, long Down);

    internal sealed class Handler(IValidator<Request> validator, IQueryProcessor queryProcessor) : IQueryHandler<Request, List<Response>>
    {
        public async Task<Result<List<Response>>> HandleAsync(Request request)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Result<List<Response>>.Failure("validation-failed");
            }

            var comments = await queryProcessor.Query<Comment>()
                .Include(c => c.Votes)
                .OrderByDescending(p => p.Votes.Where(v => v.Type == CommentVoteType.Upvote).Count() - p.Votes.Where(v => v.Type == CommentVoteType.Downvote).Count())
                .ThenByDescending(c => c.PublishDate)
                .Where(c => c.PostId == request.PostId)
                .Select(c => new Response(
                    c.Id,
                    c.Content,
                    c.PublishDate,
                    c.OwnerId,
                    c.ParentId,
                    c.Votes.Where(v => v.Type == CommentVoteType.Upvote).LongCount(),
                    c.Votes.Where(v => v.Type == CommentVoteType.Downvote).LongCount()
                ))
                .ToListAsync();

            return Result<List<Response>>.Success(comments);
        }
    }

    private sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(request => request.PostId)
                .NotEmpty();
        }
    }
}