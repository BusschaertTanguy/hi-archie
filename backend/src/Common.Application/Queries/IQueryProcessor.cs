namespace Common.Application.Queries;

public interface IQueryProcessor
{
    public IQueryable<T> Query<T>() where T : class;
}