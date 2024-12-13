using System.Net;
using Common.Application.Commands;
using Common.Application.Queries;
using Core.Application.Users.Commands;
using Core.Application.Users.Queries;
using Host.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Host.WebApi.Routes;

public static class UserRoutes
{
    public static void MapUserRoutes(this IEndpointRouteBuilder api)
    {
        var group = api.MapGroup("users")
            .WithTags("Users");

        group
            .MapGet("me", async (HttpContext context, [FromServices] IQueryHandler<GetUserByExternalId.Request, GetUserByExternalId.Response?> queryHandler,
                [FromServices] ICommandHandler<CreateUser.Command> commandHandler) =>
            {
                var subject = context.GetSubjectClaimValue();
                if (string.IsNullOrWhiteSpace(subject))
                {
                    return Results.Unauthorized();
                }

                var result = await queryHandler.HandleAsync(new(subject));

                if (result.Data == null)
                {
                    var command = new CreateUser.Command(subject);
                    await commandHandler.HandleAsync(command);
                    result = await queryHandler.HandleAsync(new(subject));
                }

                return result.ToHttpResult();
            })
            .Produces<GetUserByExternalId.Response?>()
            .Produces((int)HttpStatusCode.Unauthorized);
    }
}