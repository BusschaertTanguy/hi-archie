using System.Net;
using System.Security.Claims;
using Common.Application.Commands;
using Common.Application.Queries;
using Core.Application.Posts.Commands;
using Core.Application.Posts.Queries;
using Core.Domain.Posts.Repositories;
using Host.WebApi.Extensions;
using Host.WebApi.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Host.WebApi.Routes;

public static class PostRoutes
{
    public static void MapPostRoutes(this IEndpointRouteBuilder api)
    {
        var group = api.MapGroup("posts")
            .WithTags("Posts");

        group
            .MapGet("", async ([FromServices] IQueryHandler<GetPosts.Request, GetPosts.Response> handler, [FromQuery] Guid communityId, [FromQuery] int pageIndex, [FromQuery] int pageSize,
                [FromQuery] string? title) =>
            {
                var result = await handler.HandleAsync(new(communityId, pageIndex, pageSize, title));
                return result.ToHttpResult();
            })
            .Produces<GetPosts.Response>()
            .ProducesProblem((int)HttpStatusCode.BadRequest);

        group
            .MapGet("{id:guid}", async ([FromServices] IQueryHandler<GetPost.Request, GetPost.Response> handler,
                [FromRoute] Guid id) =>
            {
                var result = await handler.HandleAsync(new(id));
                return result.ToHttpResult();
            })
            .Produces<GetPost.Response>()
            .ProducesProblem((int)HttpStatusCode.BadRequest);

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

        group
            .MapPut("", async (
                ClaimsPrincipal user,
                [FromServices] IPostRepository postRepository,
                [FromServices] IAuthorizationService authorizationService,
                [FromServices] ICommandHandler<EditPost.Command> handler,
                [FromBody] EditPost.Command command) =>
            {
                var post = await postRepository.GetByIdAsync(command.Id);
                var authorizationResult =
                    await authorizationService.AuthorizeAsync(user, post, new UserIsOwnerRequirement());

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

        group
            .MapDelete("{id:guid}", async (
                ClaimsPrincipal user,
                [FromServices] IPostRepository postRepository,
                [FromServices] IAuthorizationService authorizationService,
                [FromServices] ICommandHandler<RemovePost.Command> handler,
                [FromRoute] Guid id) =>
            {
                var post = await postRepository.GetByIdAsync(id);
                var authorizationResult = await authorizationService.AuthorizeAsync(user, post, new UserIsOwnerRequirement());

                if (!authorizationResult.Succeeded)
                {
                    return Results.Forbid();
                }

                var result = await handler.HandleAsync(new(id));
                return result.ToHttpResult();
            })
            .Produces((int)HttpStatusCode.NoContent)
            .Produces((int)HttpStatusCode.Unauthorized)
            .Produces((int)HttpStatusCode.Forbidden)
            .ProducesProblem((int)HttpStatusCode.BadRequest)
            .RequireAuthorization();

        group
            .MapPost("vote", async ([FromServices] ICommandHandler<VotePost.Command> handler, [FromBody] VotePost.Command command) =>
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