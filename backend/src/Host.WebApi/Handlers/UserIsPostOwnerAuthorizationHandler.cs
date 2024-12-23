using Core.Domain.Posts.Entities;
using Core.Domain.Users.Repositories;
using Host.WebApi.Extensions;
using Host.WebApi.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Host.WebApi.Handlers;

internal sealed class UserIsPostOwnerAuthorizationHandler(IUserRepository userRepository) : AuthorizationHandler<UserIsOwnerRequirement, Post>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsOwnerRequirement requirement, Post resource)
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