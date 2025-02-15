﻿using System.Net;
using System.Security.Claims;
using Common.Application.Commands;
using Common.Application.Queries;
using Core.Application.Communities.Commands;
using Core.Application.Communities.Queries;
using Core.Domain.Communities.Repositories;
using Host.WebApi.Extensions;
using Host.WebApi.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Host.WebApi.Routes;

public static class CommunityRoutes
{
    public static void MapCommunityRoutes(this IEndpointRouteBuilder api)
    {
        var group = api.MapGroup("communities")
            .WithTags("Communities");

        group
            .MapGet("", async (
                [FromServices] IQueryHandler<GetCommunities.Request, GetCommunities.Response> handler,
                [FromQuery] int pageIndex,
                [FromQuery] int pageSize,
                [FromQuery] string? name) =>
            {
                var result = await handler.HandleAsync(new(pageIndex, pageSize, name));
                return result.ToHttpResult();
            })
            .Produces<GetCommunities.Response>()
            .ProducesProblem((int)HttpStatusCode.BadRequest);

        group
            .MapGet("{id:guid}",
                async (
                    [FromServices] IQueryHandler<GetCommunity.Request, GetCommunity.Response> handler,
                    [FromRoute] Guid id) =>
                {
                    var result = await handler.HandleAsync(new(id));
                    return result.ToHttpResult();
                })
            .Produces<GetCommunity.Response>()
            .ProducesProblem((int)HttpStatusCode.BadRequest);

        group
            .MapPost("",
                async (
                    [FromServices] ICommandHandler<AddCommunity.Command> handler,
                    [FromBody] AddCommunity.Command command) =>
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
            .MapPost("join",
                async (
                    [FromServices] ICommandHandler<JoinCommunity.Command> handler,
                    [FromBody] JoinCommunity.Command command) =>
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
            .MapPost("leave",
                async (
                    [FromServices] ICommandHandler<LeaveCommunity.Command> handler,
                    [FromBody] LeaveCommunity.Command command) =>
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
                [FromServices] ICommunityRepository communityRepository,
                [FromServices] IAuthorizationService authorizationService,
                [FromServices] ICommandHandler<EditCommunity.Command> handler,
                [FromBody] EditCommunity.Command command) =>
            {
                var community = await communityRepository.GetByIdAsync(command.Id);
                var authorizationResult =
                    await authorizationService.AuthorizeAsync(user, community, new UserIsOwnerRequirement());

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
                [FromServices] ICommunityRepository communityRepository,
                [FromServices] IAuthorizationService authorizationService,
                [FromServices] ICommandHandler<RemoveCommunity.Command> handler,
                [FromRoute] Guid id) =>
            {
                var community = await communityRepository.GetByIdAsync(id);
                var authorizationResult = await authorizationService.AuthorizeAsync(user, community, new UserIsOwnerRequirement());

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
    }
}