using System.Security.Claims;

namespace Host.WebApi.Extensions;

internal static class HttpContextExtensions
{
    internal static string? GetSubjectClaimValue(this HttpContext httpContext)
    {
        return httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}