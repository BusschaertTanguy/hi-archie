using System.Text.Json.Serialization;
using Common.Application.Commands;
using Common.Application.Models;
using Core.Domain.Communities.Repositories;
using FluentValidation;

namespace Core.Application.Communities.Commands;

public static class LeaveCommunity
{
    public sealed record Command(Guid CommunityId) : ICommand, IRequiredUser
    {
        [JsonIgnore]
        public Guid UserId { get; init; }
    }

    internal sealed class Handler(IValidator<Command> validator, ISubscriptionRepository subscriptionRepository) : ICommandHandler<Command>
    {
        public async Task<Result> HandleAsync(Command command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Result.Failure("validation-failed");
            }

            await subscriptionRepository.RemoveAsync(command.CommunityId, command.UserId);

            return Result.Success();
        }
    }

    private sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CommunityId)
                .NotEmpty();

            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}