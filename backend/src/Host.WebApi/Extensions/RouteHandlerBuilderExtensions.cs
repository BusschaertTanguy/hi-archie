using Common.Application.Models;

namespace Host.WebApi.Extensions;

internal static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder RequireUser(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            if (context.Arguments.FirstOrDefault(a => a?.GetType().GetInterface(nameof(IRequiredUser)) != null) is not IRequiredUser request)
            {
                throw new InvalidOperationException($"Request is not of type {nameof(IRequiredUser)}");
            }

            var userId = await context.HttpContext.GetUserId();

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var userProperty = request.GetType().GetProperty(nameof(IRequiredUser.UserId));
            if (userProperty == null)
            {
                throw new InvalidOperationException($"Property {nameof(IRequiredUser.UserId)} not found on command");
            }

            userProperty.SetValue(request, userId);
            return await next(context);
        });
    }
}