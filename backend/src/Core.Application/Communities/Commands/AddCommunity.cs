using System.Text.Json.Serialization;
using Common.Application.Commands;
using Common.Application.Models;
using Core.Domain.Communities.Entities;
using Core.Domain.Communities.Repositories;
using FluentValidation;

namespace Core.Application.Communities.Commands;

public static class AddCommunity
{
    public sealed record Command(Guid Id, string Name) : ICommand, IRequiredUser
    {
        [JsonIgnore]
        public Guid UserId { get; init; }
    }

    internal sealed class Handler(IValidator<Command> validator, ICommunityRepository communityRepository) : ICommandHandler<Command>
    {
        public async Task<Result> HandleAsync(Command command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Result.Failure("validation-failed");
            }

            var (id, name) = command;

            var community = new Community
            {
                Id = id,
                Name = name,
                OwnerId = command.UserId
            };

            await communityRepository.AddAsync(community);

            return Result.Success();
        }
    }

    private sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Id)
                .NotEmpty();

            RuleFor(c => c.Name)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(c => c.UserId)
                .NotEmpty();
        }
    }
}