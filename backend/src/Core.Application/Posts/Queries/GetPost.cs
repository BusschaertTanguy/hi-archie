using Common.Application.Models;
using Common.Application.Queries;
using Core.Domain.Posts.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Posts.Queries;

public static class GetPost
{
    public sealed record Request(Guid Id) : IQuery<Response>;

    public sealed record Response(Guid Id, string Title, string Content, DateTime PublishDate, Guid OwnerId);

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
                .Where(p => p.Id == request.Id)
                .Select(p => new Response(p.Id, p.Title, p.Content, p.PublishDate, p.OwnerId))
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