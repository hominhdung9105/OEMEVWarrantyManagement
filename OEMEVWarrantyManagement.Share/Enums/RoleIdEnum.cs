using System.ComponentModel;

namespace OEMEVWarrantyManagement.Share.Enums
{
    public enum RoleIdEnum
    {
        [Description("ADMIN")]
        Admin,
        [Description("SC_STAFF")]
        ScStaff,
        [Description("SC_TECH")]
        Technician,
        [Description("EVM_STAFF")]
        EvmStaff
    }

    public static class RoleIdEnumExtensions
    {
        public static string GetRoleId(this RoleIdEnum role)
        {
            var memberInfo = typeof(RoleIdEnum).GetField(role.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? role.ToString();
        }
    }

}
