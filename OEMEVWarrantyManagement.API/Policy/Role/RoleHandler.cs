using Microsoft.AspNetCore.Authorization;

namespace OEMEVWarrantyManagement.API.Policy.Role
{
    public class RoleHandler : AuthorizationHandler<RoleRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RoleRequirement requirement)
        {
            var roleClaim = context.User.FindFirst("role_id");

            if (roleClaim != null && roleClaim.Value == requirement.RequiredRoleId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
