using System.Security.Claims;

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
}