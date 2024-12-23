﻿using Common.Application.Commands;
using Common.Application.Models;
using Core.Domain.Communities.Repositories;
using FluentValidation;

namespace Core.Application.Communities.Commands;

public static class RemoveCommunity
{
    public sealed record Command(Guid Id) : ICommand;

    internal sealed class Handler(IValidator<Command> validator, IUnitOfWork unitOfWork, ICommunityRepository communityRepository) : ICommandHandler<Command>
    {
        public async Task<Result> HandleAsync(Command command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Result.Failure("validation-failed");
            }

            var community = await communityRepository.GetByIdAsync(command.Id);
            await communityRepository.RemoveAsync(community);
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