using Core.Domain.Comments.Entities;
using Core.Domain.Users.Repositories;
using Host.WebApi.Extensions;
using Host.WebApi.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Host.WebApi.Handlers;

internal sealed class UserIsCommentOwnerAuthorizationHandler(IUserRepository userRepository) : AuthorizationHandler<UserIsOwnerRequirement, Comment>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsOwnerRequirement requirement, Comment resource)
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