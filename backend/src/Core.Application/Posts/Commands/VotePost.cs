using System.Text.Json.Serialization;
using Common.Application.Commands;
using Common.Application.Models;
using Core.Domain.Posts.Enums;
using Core.Domain.Posts.Repositories;
using FluentValidation;

namespace Core.Application.Posts.Commands;

public static class VotePost
{
    public sealed record Command(Guid PostId, PostVoteType Type) : ICommand, IRequiredUser
    {
        [JsonIgnore]
        public Guid UserId { get; init; }
    }

    internal sealed class Handler(IValidator<Command> validator, IPostVoteRepository postVoteRepository) : ICommandHandler<Command>
    {
        public async Task<Result> HandleAsync(Command command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Result.Failure("validation-failed");
            }

            var vote = await postVoteRepository.GetByIdAsync(command.PostId, command.UserId);

            if (vote == null)
            {
                vote = new()
                {
                    PostId = command.PostId,
                    UserId = command.UserId,
                    Type = command.Type
                };

                await postVoteRepository.AddAsync(vote);
            }
            else
            {
                if (vote.Type == command.Type)
                {
                    await postVoteRepository.RemoveAsync(vote.PostId, vote.UserId);
                }
                else
                {
                    vote.Type = command.Type;
                    await postVoteRepository.UpdateAsync(vote);
                }
            }

            return Result.Success();
        }
    }

    private sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.PostId)
                .NotEmpty();

            RuleFor(c => c.Type)
                .IsInEnum();

            RuleFor(c => c.UserId)
                .NotEmpty();
        }
    }
}