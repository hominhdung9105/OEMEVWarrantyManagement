using System.ComponentModel;

namespace OEMEVWarrantyManagement.Share.Enums
{
    public enum ClaimPartStatus
    {
        [Description("Pending")]
        Pending,

        [Description("Not enough")]
        NotEnough,

        [Description("Enough")]
        Enough,

        [Description("Installed")]
        Installed
    }

    public static class ClaimPartStatusExtensions
    {
        public static string GetClaimPartStatus(this ClaimPartStatus status)
        {
            var memberInfo = typeof(PartCategory).GetField(status.ToString());
            return ((DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute))).ToString();
        }
    }

    public enum ClaimPartAction
    {
        [Description("Replace")]
        Replace,

        [Description("Repair")]
        Repair
    }

    public static class ClaimPartActionExtensions
    {
        public static string GetClaimPartAction(this ClaimPartAction action)
        {
            var memberInfo = typeof(PartCategory).GetField(action.ToString());
            return ((DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute))).ToString();
        }
    }
}
