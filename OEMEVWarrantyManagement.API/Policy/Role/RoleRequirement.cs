using Microsoft.AspNetCore.Authorization;

namespace OEMEVWarrantyManagement.API.Policy.Role
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public string RequiredRoleId { get; }

        public RoleRequirement(string roleId)
        {
            RequiredRoleId = roleId;
        }
    }
}
