using Common.Application.Models;
using Common.Application.Queries;
using Core.Domain.Communities.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Communities.Queries;

public static class GetCommunity
{
    public sealed record Request(Guid Id) : IQuery<Response>;

    public sealed record Response(Guid Id, string Name, Guid OwnerId);

    internal sealed class Handler(IValidator<Request> validator, IQueryProcessor queryProcessor) : IQueryHandler<Request, Response>
    {
        public async Task<Result<Response>> HandleAsync(Request request)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Result<Response>.Failure("validation-failed");
            }

            var response = await queryProcessor.Query<Community>()
                .Where(c => c.Id == request.Id)
                .Select(c => new Response(c.Id, c.Name, c.OwnerId))
                .FirstAsync();

            return Result<Response>.Success(response);
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