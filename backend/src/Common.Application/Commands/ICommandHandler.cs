using Common.Application.Models;

namespace Common.Application.Commands;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command);
}