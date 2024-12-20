using System.Net;
using Common.Application.Commands;
using Core.Application.Posts.Commands;
using Host.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Host.WebApi.Routes;

public static class PostRoutes
{
    public static void MapPostRoutes(this IEndpointRouteBuilder api)
    {
        var group = api.MapGroup("posts")
            .WithTags("Posts");

        group
            .MapPost("",
                async ([FromServices] ICommandHandler<AddPost.Command> handler,
                    [FromBody] AddPost.Command command) =>
                {
                    var result = await handler.HandleAsync(command);
                    return result.ToHttpResult();
                })
            .Produces((int)HttpStatusCode.NoContent)
            .Produces((int)HttpStatusCode.Unauthorized)
            .ProducesProblem((int)HttpStatusCode.BadRequest)
            .RequireAuthorization()
            .RequireUser();
    }
}