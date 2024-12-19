using Common.Application.Commands;
using Common.Application.Models;
using Core.Domain.Users.Entities;
using Core.Domain.Users.Repositories;
using FluentValidation;

namespace Core.Application.Users.Commands;

public static class CreateUser
{
    public sealed record Command(string ExternalId) : ICommand;

    internal sealed class Handler(IValidator<Command> validator, IUnitOfWork unitOfWork, IUserRepository userRepository)
        : ICommandHandler<Command>
    {
        public async Task<Result> HandleAsync(Command command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Result.Failure("validation-failed");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                ExternalId = command.ExternalId
            };

            await userRepository.AddAsync(user);
            await unitOfWork.CommitAsync();

            return Result.Success();
        }
    }

    private sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.ExternalId).NotEmpty();
        }
    }
}