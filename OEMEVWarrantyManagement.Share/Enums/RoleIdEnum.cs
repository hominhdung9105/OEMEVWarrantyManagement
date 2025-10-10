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
        public static string GetRoleId(this RoleIdEnum error)
        {
            var memberInfo = typeof(RoleIdEnum).GetField(error.ToString());
            return ((DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute))).ToString();
        }
    }
}
