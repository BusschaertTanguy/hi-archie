using System.Text.Json.Serialization;
using Common.Application.Commands;
using Common.Application.Models;
using Core.Domain.Communities.Entities;
using Core.Domain.Communities.Repositories;
using FluentValidation;

namespace Core.Application.Communities.Commands;

public static class JoinCommunity
{
    public sealed record Command(Guid CommunityId) : ICommand, IRequiredUser
    {
        [JsonIgnore]
        public Guid UserId { get; init; }
    }

    internal sealed class Handler(IValidator<Command> validator, IUnitOfWork unitOfWork, ISubscriptionRepository subscriptionRepository) : ICommandHandler<Command>
    {
        public async Task<Result> HandleAsync(Command command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Result.Failure("validation-failed");
            }

            var exists = await subscriptionRepository.ExistsAsync(command.CommunityId, command.UserId);
            if (exists)
            {
                return Result.Success();
            }

            var subscription = new Subscription
            {
                CommunityId = command.CommunityId,
                UserId = command.UserId
            };

            await subscriptionRepository.AddAsync(subscription);
            await unitOfWork.CommitAsync();

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