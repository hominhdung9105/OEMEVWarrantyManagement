namespace OEMEVWarrantyManagement.Share.Enum
{
    public enum RoleIdEnum
    {
        [RoleIdAttr("ROL-ADMIN")]
        Admin,
        [RoleIdAttr("ROL-STAFF")]
        ScStaff,
        [RoleIdAttr("ROL-TECH")]
        Technician,
        [RoleIdAttr("ROL-EVM")]
        EvmStaff
    }

    public static class RoleIdEnumExtensions
    {
        public static string GetRoleId(this RoleIdEnum error)
        {
            var memberInfo = typeof(RoleIdEnum).GetField(error.ToString());
            return ((RoleIdAttr)Attribute.GetCustomAttribute(memberInfo, typeof(RoleIdAttr))).Id;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RoleIdAttr(string id) : Attribute
    {
        public string Id { get; } = id;
    }
}
