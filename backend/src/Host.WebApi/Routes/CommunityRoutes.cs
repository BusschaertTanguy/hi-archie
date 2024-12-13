using System.Net;
using Common.Application.Commands;
using Common.Application.Queries;
using Core.Application.Communities.Commands;
using Core.Application.Communities.Queries;
using Host.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Host.WebApi.Routes;

public static class CommunityRoutes
{
    public static void MapCommunityRoutes(this IEndpointRouteBuilder api)
    {
        var group = api.MapGroup("communities")
            .WithTags("Communities");

        group
            .MapGet("", async ([FromServices] IQueryHandler<GetCommunities.Request, GetCommunities.Response> handler, [FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? name) =>
            {
                var result = await handler.HandleAsync(new(pageIndex, pageSize, name));
                return result.ToHttpResult();
            })
            .Produces<GetCommunities.Response>()
            .ProducesProblem((int)HttpStatusCode.BadRequest);

        group
            .MapPost("", async ([FromServices] ICommandHandler<AddCommunity.Command> handler, [FromBody] AddCommunity.Command command) =>
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