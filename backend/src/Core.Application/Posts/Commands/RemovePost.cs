using Common.Application.Commands;
using Common.Application.Models;
using Common.Application.Queues;
using Core.Application.Posts.Events;
using Core.Domain.Posts.Repositories;
using FluentValidation;

namespace Core.Application.Posts.Commands;

public static class RemovePost
{
    public sealed record Command(Guid Id) : ICommand;

    internal sealed class Handler(IValidator<Command> validator, IPostRepository postRepository, IAsyncQueue asyncQueue) : ICommandHandler<Command>
    {
        public async Task<Result> HandleAsync(Command command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Result.Failure("validation-failed");
            }

            await postRepository.RemoveAsync(command.Id);
            await asyncQueue.PublishAsync(new PostRemoved
            {
                Id = command.Id
            });

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