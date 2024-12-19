using Microsoft.AspNetCore.Authorization;

namespace Host.WebApi.Requirements;

internal sealed class UserIsOwnerRequirement : IAuthorizationRequirement;