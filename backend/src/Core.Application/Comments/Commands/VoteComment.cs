using System.Text.Json.Serialization;
using Common.Application.Commands;
using Common.Application.Models;
using Common.Application.Queues;
using Core.Application.Comments.Events;
using Core.Domain.Comments.Enums;
using Core.Domain.Comments.Repositories;
using FluentValidation;

namespace Core.Application.Comments.Commands;

public static class VoteComment
{
    public sealed record Command(Guid CommentId, CommentVoteType Type) : ICommand, IRequiredUser
    {
        [JsonIgnore]
        public Guid UserId { get; init; }
    }

    internal sealed class Handler(IValidator<Command> validator, ICommentVoteRepository commentVoteRepository, IAsyncQueue asyncQueue) : ICommandHandler<Command>
    {
        public async Task<Result> HandleAsync(Command command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Result.Failure("validation-failed");
            }

            var vote = await commentVoteRepository.GetByIdAsync(command.CommentId, command.UserId);

            if (vote == null)
            {
                vote = new()
                {
                    CommentId = command.CommentId,
                    UserId = command.UserId,
                    Type = command.Type
                };

                await commentVoteRepository.AddAsync(vote);
                await asyncQueue.PublishAsync(CommentVoted.QueueName, new CommentVoted
                {
                    Id = vote.CommentId,
                    UpChange = vote.Type == CommentVoteType.Upvote ? 1 : 0,
                    DownChange = vote.Type == CommentVoteType.Downvote ? 1 : 0
                });
                
                return Result.Success();
            }
            
            if (vote.Type == command.Type)
            {
                await commentVoteRepository.RemoveAsync(vote.CommentId, command.UserId);
                await asyncQueue.PublishAsync(CommentVoted.QueueName, new CommentVoted
                {
                    Id = vote.CommentId,
                    UpChange = vote.Type == CommentVoteType.Upvote ? -1 : 0,
                    DownChange = vote.Type == CommentVoteType.Downvote ? -1 : 0
                });
                
                return Result.Success();
            }
            
            vote.Type = command.Type;
            
            await commentVoteRepository.UpdateAsync(vote);
            await asyncQueue.PublishAsync(CommentVoted.QueueName, new CommentVoted
            {
                Id = vote.CommentId,
                UpChange = vote.Type == CommentVoteType.Upvote ? 1 : -1,
                DownChange = vote.Type == CommentVoteType.Downvote ? 1 : -1
            });

            return Result.Success();
        }
    }

    private sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.CommentId)
                .NotEmpty();

            RuleFor(c => c.Type)
                .IsInEnum();

            RuleFor(c => c.UserId)
                .NotEmpty();
        }
    }
}