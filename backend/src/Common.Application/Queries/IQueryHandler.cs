using Common.Application.Models;

namespace Common.Application.Queries;

public interface IQueryHandler<in TRequest, TResponse> where TRequest : IQuery<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TRequest request);
}