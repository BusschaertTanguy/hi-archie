using System.Net;
using Common.Application.Models;

namespace Host.WebApi.Extensions;

internal static class ResultExtensions
{
    internal static IResult ToHttpResult(this Result result)
    {
        return result.Error switch
        {
            null => TypedResults.NoContent(),
            _ => TypedResults.Problem(result.Error, statusCode: (int)HttpStatusCode.BadRequest)
        };
    }

    internal static IResult ToHttpResult<TData>(this Result<TData> result)
    {
        if (result.Error is not null)
        {
            return TypedResults.Problem(result.Error, statusCode: (int)HttpStatusCode.BadRequest);
        }

        return result.Data is not null ? TypedResults.Ok(result.Data) : TypedResults.NoContent();
    }
}