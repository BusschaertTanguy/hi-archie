using System.Text.Json.Serialization;
using Common.Application.Commands;
using Common.Application.Models;
using Common.Application.Queues;
using Core.Application.Comments.Events;
using Core.Domain.Comments.Entities;
using Core.Domain.Comments.Repositories;
using FluentValidation;

namespace Core.Application.Comments.Commands;

public static class AddComment
{
    public sealed record Command(Guid Id, string Content, Guid PostId, DateTime PublishDate, Guid? ParentId) : ICommand, IRequiredUser
    {
        [JsonIgnore]
        public Guid UserId { get; init; }
    }

    internal sealed class Handler(IValidator<Command> validator, ICommentRepository commentRepository, IAsyncQueue asyncQueue) : ICommandHandler<Command>
    {
        public async Task<Result> HandleAsync(Command command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Result.Failure("validation-failed");
            }

            var (id, content, postId, publishDate, parentId) = command;

            var comment = new Comment
            {
                Id = id,
                Content = content,
                PostId = postId,
                OwnerId = command.UserId,
                PublishDate = publishDate,
                ParentId = parentId
            };

            await commentRepository.AddAsync(comment);
            await asyncQueue.PublishAsync(CommentAdded.QueueName, new CommentAdded { Id = comment.Id });

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

            RuleFor(c => c.PostId)
                .NotEmpty();

            RuleFor(c => c.UserId)
                .NotEmpty();
        }
    }
}