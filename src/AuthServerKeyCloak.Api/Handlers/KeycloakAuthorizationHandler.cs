using Microsoft.AspNetCore.Authorization;

namespace AuthServerKeyCloak.Api.Handlers
{
    public class RealmAdminRequirement : IAuthorizationRequirement { }

    public class RealmAdminHandler : AuthorizationHandler<RealmAdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RealmAdminRequirement requirement)
        {
            var resourceAccess = context.User.FindFirst("resource_access")?.Value;
            if (!string.IsNullOrEmpty(resourceAccess))
            {
                var realmAccess = System.Text.Json.JsonDocument.Parse(resourceAccess);
                if (realmAccess.RootElement.TryGetProperty("realm-management", out var realmMgmt))
                {
                    if (realmMgmt.TryGetProperty("roles", out var realmRoles))
                    {
                        if (realmRoles.EnumerateArray().Any(r => r.GetString() == "manage-realm"))
                        {
                            context.Succeed(requirement);
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
