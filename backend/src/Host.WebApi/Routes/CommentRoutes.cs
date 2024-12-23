using System.Net;
using Common.Application.Commands;
using Common.Application.Queries;
using Core.Application.Comments.Commands;
using Core.Application.Comments.Queries;
using Host.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Host.WebApi.Routes;

public static class CommentRoutes
{
    public static void MapCommentRoutes(this IEndpointRouteBuilder api)
    {
        var group = api.MapGroup("comments")
            .WithTags("Comments");

        group
            .MapGet("", async ([FromServices] IQueryHandler<GetComments.Request, List<GetComments.Response>> handler, [FromQuery] Guid postId) =>
            {
                var result = await handler.HandleAsync(new(postId));
                return result.ToHttpResult();
            })
            .Produces<List<GetComments.Response>>()
            .ProducesProblem((int)HttpStatusCode.BadRequest);

        group
            .MapPost("",
                async ([FromServices] ICommandHandler<AddComment.Command> handler,
                    [FromBody] AddComment.Command command) =>
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