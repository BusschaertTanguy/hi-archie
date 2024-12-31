using System.Text.Json.Serialization;
using Common.Application.Commands;
using Common.Application.Models;
using Common.Application.Queues;
using Core.Application.Posts.Events;
using Core.Domain.Communities.Repositories;
using Core.Domain.Posts.Entities;
using Core.Domain.Posts.Repositories;
using FluentValidation;

namespace Core.Application.Posts.Commands;

public static class AddPost
{
    public sealed record Command(Guid Id, Guid CommunityId, string Title, string Content) : ICommand, IRequiredUser
    {
        [JsonIgnore]
        public Guid UserId { get; init; }
    }

    internal sealed class Handler(IValidator<Command> validator, IPostRepository postRepository, ISubscriptionRepository subscriptionRepository, IAsyncQueue asyncQueue) : ICommandHandler<Command>
    {
        public async Task<Result> HandleAsync(Command command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Result.Failure("validation-failed");
            }

            var exist = await subscriptionRepository.ExistsAsync(command.CommunityId, command.UserId);
            if (!exist)
            {
                return Result.Failure("not-subscribed");
            }

            var (id, communityId, title, content) = command;

            var post = new Post
            {
                Id = id,
                CommunityId = communityId,
                Title = title,
                Content = content,
                PublishDate = DateTime.UtcNow,
                OwnerId = command.UserId
            };

            await postRepository.AddAsync(post);
            await asyncQueue.PublishAsync(PostAdded.QueueName, new PostAdded { Id = post.Id });

            return Result.Success();
        }
    }

    private sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Id)
                .NotEmpty();

            RuleFor(c => c.CommunityId)
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