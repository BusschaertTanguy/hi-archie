namespace Common.Application.Commands;

public interface IUnitOfWork
{
    Task CommitAsync();
}