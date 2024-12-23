using Core.Domain.Communities.Entities;
using Core.Domain.Users.Repositories;
using Host.WebApi.Extensions;
using Host.WebApi.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Host.WebApi.Handlers;

internal sealed class UserIsCommunityOwnerAuthorizationHandler(IUserRepository userRepository) : AuthorizationHandler<UserIsOwnerRequirement, Community>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsOwnerRequirement requirement, Community resource)
    {
        var externalId = context.User.GetSubjectClaimValue();
        if (string.IsNullOrWhiteSpace(externalId))
        {
            return;
        }

        var user = await userRepository.GetUserAsync(externalId);
        if (user == null)
        {
            return;
        }

        if (user.Id == resource.OwnerId)
        {
            context.Succeed(requirement);
        }
    }
}