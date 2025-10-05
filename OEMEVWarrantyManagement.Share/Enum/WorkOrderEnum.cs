namespace OEMEVWarrantyManagement.Share.Enum
{
    public enum WorkOrderType
    {
        [TypeAttr("Inspection")]
        Inspection,

        [TypeAttr("Repair")]
        Repair
    }

    public static class WorkOrderTypeExtensions
    {
        public static string GetWorkOrderType(this WorkOrderType type)
        {
            var memberInfo = typeof(WorkOrderType).GetField(type.ToString());
            return ((TypeAttr)Attribute.GetCustomAttribute(memberInfo, typeof(TypeAttr))).Type;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class TypeAttr(string type) : Attribute
    {
        public string Type { get; } = type;
    }

    public enum WorkOrderTarget
    {
        [TargetAttr("Warranty")]
        Warranty,

        [TargetAttr("Campaign")]
        Campaign
    }

    public static class WorkOrderTargetExtensions
    {
        public static string GetWorkOrderTarget(this WorkOrderTarget target)
        {
            var memberInfo = typeof(WorkOrderTarget).GetField(target.ToString());
            return ((TargetAttr)Attribute.GetCustomAttribute(memberInfo, typeof(TargetAttr))).Target;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class TargetAttr(string target) : Attribute
    {
        public string Target { get; } = target;
    }

    public enum WorkOrderStatus
    {
        [StatusAttr("in progress")]
        InProgress,
        [StatusAttr("completed")]
        Completed
    }

    public static class WorkOrderStatusExtensions
    {
        public static string GetWorkOrderStatus(this WorkOrderStatus status)
        {
            var memberInfo = typeof(WorkOrderStatus).GetField(status.ToString());
            return ((StatusAttr)Attribute.GetCustomAttribute(memberInfo, typeof(StatusAttr))).Status;
        }
    }
}
