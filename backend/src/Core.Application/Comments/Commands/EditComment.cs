using Common.Application.Commands;
using Common.Application.Models;
using Common.Application.Queues;
using Core.Application.Comments.Events;
using Core.Domain.Comments.Repositories;
using FluentValidation;

namespace Core.Application.Comments.Commands;

public static class EditComment
{
    public sealed record Command(Guid Id, string Content) : ICommand;

    internal sealed class Handler(IValidator<Command> validator, ICommentRepository commentRepository, IAsyncQueue asyncQueue) : ICommandHandler<Command>
    {
        public async Task<Result> HandleAsync(Command command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Result.Failure("validation-failed");
            }

            var (id, content) = command;

            var comment = await commentRepository.GetByIdAsync(id);

            comment.Content = content;

            await commentRepository.UpdateAsync(comment);
            await asyncQueue.PublishAsync(CommentEdited.QueueName, new CommentEdited { Id = comment.Id });

            return Result.Success();
        }
    }

    private sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Id)
                .NotEmpty();

            RuleFor(c => c.Content)
                .NotEmpty()
                .MaximumLength(10_000);
        }
    }
}