using Common.Application.Commands;
using Common.Application.Models;
using Core.Domain.Posts.Repositories;
using FluentValidation;

namespace Core.Application.Posts.Commands;

public static class EditPost
{
    public sealed record Command(Guid Id, string Title, string Content) : ICommand;

    internal sealed class Handler(IValidator<Command> validator, IUnitOfWork unitOfWork, IPostRepository postRepository) : ICommandHandler<Command>
    {
        public async Task<Result> HandleAsync(Command command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Result.Failure("validation-failed");
            }

            var (id, title, content) = command;

            var post = await postRepository.GetByIdAsync(id);

            post.Title = title;
            post.Content = content;

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

            RuleFor(c => c.Title)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(10_000);
        }
    }
}