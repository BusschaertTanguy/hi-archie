using System.Security.Claims;
using Common.Application.Queries;
using Core.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Host.WebApi.Extensions;

internal static class HttpContextExtensions
{
    internal static string? GetSubjectClaimValue(this HttpContext httpContext)
    {
        return httpContext.User.GetSubjectClaimValue();
    }

    internal static string? GetSubjectClaimValue(this ClaimsPrincipal principal)
    {
        return principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }

    internal static async Task<Guid?> GetUserId(this HttpContext httpContext)
    {
        var subject = httpContext.GetSubjectClaimValue();

        var queryHandler = httpContext.RequestServices
            .GetRequiredService<IQueryProcessor>();

        var user = await queryHandler.Query<User>()
            .Where(u => u.ExternalId == subject)
            .Select(u => new { u.Id })
            .FirstOrDefaultAsync();

        return user?.Id;
    }
}