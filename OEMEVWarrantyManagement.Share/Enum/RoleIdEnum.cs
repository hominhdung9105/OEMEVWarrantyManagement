namespace OEMEVWarrantyManagement.Share.Enum
{
    public enum RoleIdEnum
    {
        [RoleIdAttr("ADMIN")]
        Admin,
        [RoleIdAttr("SC_STAFF")]
        ScStaff,
        [RoleIdAttr("SC_TECH")]
        Technician,
        [RoleIdAttr("EVM_STAFF")]
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
