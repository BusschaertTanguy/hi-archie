using Common.Application.Commands;
using Common.Application.Models;
using Core.Domain.Posts.Repositories;
using FluentValidation;

namespace Core.Application.Posts.Commands;

public static class RemovePost
{
    public sealed record Command(Guid Id) : ICommand;

    internal sealed class Handler(IValidator<Command> validator, IUnitOfWork unitOfWork, IPostRepository postRepository) : ICommandHandler<Command>
    {
        public async Task<Result> HandleAsync(Command command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Result.Failure("validation-failed");
            }

            var post = await postRepository.GetByIdAsync(command.Id);
            await postRepository.RemoveAsync(post);
            await unitOfWork.CommitAsync();

            return Result.Success();
        }
    }

    private sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}