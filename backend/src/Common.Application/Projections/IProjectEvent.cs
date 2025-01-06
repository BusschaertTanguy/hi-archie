namespace Common.Application.Projections;

public interface IProjectEvent<in T>
{
    Task HandleAsync(T @event);
}