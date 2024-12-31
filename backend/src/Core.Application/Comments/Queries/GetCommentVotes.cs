using Common.Application.Models;
using Common.Application.Queries;
using Core.Domain.Comments.Entities;
using Core.Domain.Comments.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Comments.Queries;

public static class GetCommentVotes
{
    public sealed record Request(Guid PostId, Guid UserId) : IQuery<List<Response>>;

    public sealed class Response
    {
        public required Guid CommentId { get; init; }
        public required CommentVoteType Type { get; init; }
    }

    internal sealed class Handler(IValidator<Request> validator, IQueryProcessor queryProcessor) : IQueryHandler<Request, List<Response>>
    {
        public async Task<Result<List<Response>>> HandleAsync(Request request)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Result<List<Response>>.Failure("validation-failed");
            }

            var commentsQuery = queryProcessor.Query<Comment>()
                .Where(c => c.PostId == request.PostId)
                .Select(c => c.Id);

            var votes = await queryProcessor.Query<CommentVote>()
                .Where(cv => commentsQuery.Contains(cv.CommentId) && cv.UserId == request.UserId)
                .Select(cv => new Response { CommentId = cv.CommentId, Type = cv.Type })
                .ToListAsync();

            return Result<List<Response>>.Success(votes);
        }
    }

    private sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.PostId)
                .NotEmpty();

            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}