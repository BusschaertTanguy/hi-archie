using System.Net;
using System.Security.Claims;
using Common.Application.Commands;
using Common.Application.Queries;
using Core.Application.Comments.Commands;
using Core.Application.Comments.Queries;
using Core.Domain.Comments.Repositories;
using Host.WebApi.Extensions;
using Host.WebApi.Requirements;
using Microsoft.AspNetCore.Authorization;
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

        group
            .MapPut("", async (
                ClaimsPrincipal user,
                [FromServices] ICommentRepository commentRepository,
                [FromServices] IAuthorizationService authorizationService,
                [FromServices] ICommandHandler<EditComment.Command> handler,
                [FromBody] EditComment.Command command) =>
            {
                var comment = await commentRepository.GetByIdAsync(command.Id);
                var authorizationResult = await authorizationService.AuthorizeAsync(user, comment, new UserIsOwnerRequirement());

                if (!authorizationResult.Succeeded)
                {
                    return Results.Forbid();
                }

                var result = await handler.HandleAsync(command);
                return result.ToHttpResult();
            })
            .Produces((int)HttpStatusCode.NoContent)
            .Produces((int)HttpStatusCode.Unauthorized)
            .Produces((int)HttpStatusCode.Forbidden)
            .ProducesProblem((int)HttpStatusCode.BadRequest)
            .RequireAuthorization();
    }
}