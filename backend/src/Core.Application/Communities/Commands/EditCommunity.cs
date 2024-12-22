using Common.Application.Commands;
using Common.Application.Models;
using Core.Domain.Communities.Repositories;
using FluentValidation;

namespace Core.Application.Communities.Commands;

public static class EditCommunity
{
    public sealed record Command(Guid Id, string Name) : ICommand;

    internal sealed class Handler(
        IValidator<Command> validator,
        IUnitOfWork unitOfWork,
        ICommunityRepository communityRepository) : ICommandHandler<Command>
    {
        public async Task<Result> HandleAsync(Command command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Result.Failure("validation-failed");
            }

            var (id, name) = command;

            var community = await communityRepository.GetById(id);

            community.Name = name;

            await unitOfWork.CommitAsync();

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
        }
    }
}