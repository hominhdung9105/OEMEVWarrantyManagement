using System.ComponentModel;

namespace OEMEVWarrantyManagement.Share.Enums
{
    public enum PartOrderStatus
    {
        [Description("Pending")]
        Pending,

        [Description("Waiting")]
        Waiting,

        [Description("Confirmed")]
        Confirm,

        [Description("In Transit")]
        InTransit,

        [Description("Delivered")]
        Delivery,

        [Description("Done")]
        Done
    }

    public static class PartOrderStatusExtensions
    {
        public static string GetPartOrderStatus(this PartOrderStatus status)
        {
            var memberInfo = typeof(PartOrderStatus).GetField(status.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? status.ToString();
        }
    }
}
