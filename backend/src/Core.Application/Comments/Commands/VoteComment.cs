using System.Text.Json.Serialization;
using Common.Application.Commands;
using Common.Application.Models;
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

    internal sealed class Handler(IValidator<Command> validator, IUnitOfWork unitOfWork, ICommentVoteRepository commentVoteRepository) : ICommandHandler<Command>
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
            }
            else
            {
                if (vote.Type == command.Type)
                {
                    await commentVoteRepository.RemoveAsync(vote);
                }
                else
                {
                    vote.Type = command.Type;
                }
            }

            await unitOfWork.CommitAsync();

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