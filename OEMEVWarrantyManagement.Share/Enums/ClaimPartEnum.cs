using System;
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

        [Description("Done")]
        Done
    }

    public static class ClaimPartStatusExtensions
    {
        public static string GetClaimPartStatus(this ClaimPartStatus status)
        {
            var memberInfo = typeof(ClaimPartStatus).GetField(status.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? status.ToString();
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
            var memberInfo = typeof(ClaimPartAction).GetField(action.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? action.ToString();
        }
    }
}
