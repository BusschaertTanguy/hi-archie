using Common.Application.Models;
using Common.Application.Queries;
using Core.Application.Users.Queries;

namespace Host.WebApi.Extensions;

internal static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder RequireUser(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            if (context.Arguments.FirstOrDefault(a => a?.GetType().GetInterface(nameof(IRequiredUser)) != null) is not IRequiredUser command)
            {
                throw new InvalidOperationException($"Command is not of type {nameof(IRequiredUser)}");
            }

            var queryHandler = context.HttpContext.RequestServices.GetRequiredService<IQueryHandler<GetUserByExternalId.Request, GetUserByExternalId.Response?>>();

            var subject = context.HttpContext.GetSubjectClaimValue();
            if (subject == null)
            {
                return Results.Unauthorized();
            }

            var userResult = await queryHandler.HandleAsync(new(subject));
            var user = userResult.Data;
            if (user == null)
            {
                return Results.Unauthorized();
            }

            var userProperty = command.GetType().GetProperty(nameof(IRequiredUser.UserId));
            if (userProperty == null)
            {
                throw new InvalidOperationException($"Property {nameof(IRequiredUser.UserId)} not found on command");
            }

            userProperty.SetValue(command, user.Id);
            return await next(context);
        });
    }
}