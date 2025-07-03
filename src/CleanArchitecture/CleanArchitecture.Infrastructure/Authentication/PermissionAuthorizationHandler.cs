using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CleanArchitecture.Infrastructure.Authentication;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        string? userId = context.User.Claims.FirstOrDefault(
            c => c.Type == ClaimTypes.NameIdentifier
            )?.Value;

        if (userId == null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        HashSet<string> permissions = context.User.Claims
        .Where(c => c.Type == CustomClaims.Permissions)
            .Select(c => c.Value).ToHashSet();

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
