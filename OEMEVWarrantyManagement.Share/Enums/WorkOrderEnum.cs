using System.ComponentModel;

namespace OEMEVWarrantyManagement.Share.Enums
{
    public enum WorkOrderType
    {
        [Description("Inspection")]
        Inspection,

        [Description("Repair")]
        Repair
    }

    public static class WorkOrderTypeExtensions
    {
        public static string GetWorkOrderType(this WorkOrderType type)
        {
            var memberInfo = typeof(WorkOrderType).GetField(type.ToString());
            return ((DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute))).ToString();
        }
    }

    public enum WorkOrderTarget
    {
        [Description("Warranty")]
        Warranty,

        [Description("Campaign")]
        Campaign
    }

    public static class WorkOrderTargetExtensions
    {
        public static string GetWorkOrderTarget(this WorkOrderTarget target)
        {
            var memberInfo = typeof(WorkOrderTarget).GetField(target.ToString());
            return ((DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute))).ToString();
        }
    }

    public enum WorkOrderStatus
    {
        [Description("in progress")]
        InProgress,
        [Description("completed")]
        Completed
    }

    public static class WorkOrderStatusExtensions
    {
        public static string GetWorkOrderStatus(this WorkOrderStatus status)
        {
            var memberInfo = typeof(WorkOrderStatus).GetField(status.ToString());
            return ((DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute))).ToString();
        }
    }
}
