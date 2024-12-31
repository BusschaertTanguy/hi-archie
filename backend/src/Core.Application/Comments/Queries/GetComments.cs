using Common.Application.Models;
using Common.Application.Queries;
using Core.Application.Comments.Projections;
using FluentValidation;

namespace Core.Application.Comments.Queries;

public static class GetComments
{
    public sealed record Request(Guid PostId) : IQuery<List<CommentProjection>>;


    internal sealed class Handler(IValidator<Request> validator, ICommentProjectionReader commentProjectionReader) : IQueryHandler<Request, List<CommentProjection>>
    {
        public async Task<Result<List<CommentProjection>>> HandleAsync(Request request)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Result<List<CommentProjection>>.Failure("validation-failed");
            }

            var comments = await commentProjectionReader.GetCommentsAsync(request.PostId);
            return Result<List<CommentProjection>>.Success(comments);
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