using System.ComponentModel;

namespace OEMEVWarrantyManagement.Share.Enums
{
    /// <summary>
    /// Lý do H?Y lô hàng (m?t h?t, không quay v?)
    /// </summary>
    public enum PartOrderCancellationReason
    {
        [Description("Traffic Accident/Total Damage")]
        AccidentTotalDamage,

        [Description("Theft/Total Loss")]
        TheftOrLost,

        [Description("Fire/Explosion")]
        FireExplosion,

        [Description("Natural Disaster")]
        NaturalDisaster,

        [Description("Other Reason")]
        Other
    }

    /// <summary>
    /// Lý do TR? hàng v? (hàng quay v? kho EVM)
    /// </summary>
    public enum PartOrderReturnReason
    {
        [Description("Undeliverable/Delivery Refused")]
        DeliveryRefused,

        [Description("Wrong Address/Unreachable")]
        WrongAddressUnreachable,

        [Description("Service Center Temporarily Closed")]
        ServiceCenterClosed,

        [Description("Cancelled by Service Center")]
        CancelledBySC,

        [Description("Other Reason")]
        Other
    }

    public static class PartOrderCancellationReasonExtensions
    {
        public static string GetDescription(this PartOrderCancellationReason reason)
        {
            var memberInfo = typeof(PartOrderCancellationReason).GetField(reason.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? reason.ToString();
        }

        public static List<string> GetAllReasons()
        {
            return Enum.GetValues<PartOrderCancellationReason>()
                .Select(r => r.GetDescription())
                .ToList();
        }
    }

    public static class PartOrderReturnReasonExtensions
    {
        public static string GetDescription(this PartOrderReturnReason reason)
        {
            var memberInfo = typeof(PartOrderReturnReason).GetField(reason.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? reason.ToString();
        }

        public static List<string> GetAllReasons()
        {
            return Enum.GetValues<PartOrderReturnReason>()
                .Select(r => r.GetDescription())
                .ToList();
        }
    }

    /// <summary>
    /// Tr?ng thái quy?t ??nh c?a admin v? sai l?ch
    /// </summary>
    public enum DiscrepancyResolutionStatus
    {
        [Description("Pending Resolution")]
        PendingResolution,

        [Description("Resolved")]
        Resolved
    }

    public static class DiscrepancyResolutionStatusExtensions
    {
        public static string GetStatus(this DiscrepancyResolutionStatus status)
        {
            var memberInfo = typeof(DiscrepancyResolutionStatus).GetField(status.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? status.ToString();
        }
    }

    /// <summary>
    /// Lo?i sai l?ch
    /// </summary>
    public enum DiscrepancyType
    {
        [Description("Damaged")]
        Damaged,

        [Description("Excess")]
        Excess,

        [Description("Shortage")]
        Shortage
    }

    public static class DiscrepancyTypeExtensions
    {
        public static string GetDescription(this DiscrepancyType type)
        {
            var memberInfo = typeof(DiscrepancyType).GetField(type.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? type.ToString();
        }

        public static List<string> GetAllTypes()
        {
            return Enum.GetValues<DiscrepancyType>()
                .Select(t => t.GetDescription())
                .ToList();
        }
    }

    /// <summary>
    /// Bên ch?u trách nhi?m
    /// </summary>
    public enum ResponsibleParty
    {
        [Description("EVM")]
        EVM,

        [Description("SC")]
        SC,

        [Description("Transport")]
        Transport,

        [Description("Shared")]
        Shared
    }

    public static class ResponsiblePartyExtensions
    {
        public static string GetDescription(this ResponsibleParty party)
        {
            var memberInfo = typeof(ResponsibleParty).GetField(party.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? party.ToString();
        }

        public static List<string> GetAllParties()
        {
            return Enum.GetValues<ResponsibleParty>()
                .Select(p => p.GetDescription())
                .ToList();
        }

        public static bool IsValid(string party)
        {
            return GetAllParties().Any(p => string.Equals(p, party, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// Hành ??ng x? lý cho ph? tùng h? h?ng
    /// </summary>
    public enum DamagedPartAction
    {
        [Description("Compensate")]
        Compensate,

        [Description("Repair")]
        Repair,

        [Description("Accept As Is")]
        AcceptAsIs,

        [Description("Return To EVM")]
        ReturnToEVM
    }

    public static class DamagedPartActionExtensions
    {
        public static string GetDescription(this DamagedPartAction action)
        {
            var memberInfo = typeof(DamagedPartAction).GetField(action.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? action.ToString();
        }

        public static List<string> GetAllActions()
        {
            return Enum.GetValues<DamagedPartAction>()
                .Select(a => a.GetDescription())
                .ToList();
        }
    }

    /// <summary>
    /// Hành ??ng x? lý cho ph? tùng d?
    /// </summary>
    public enum ExcessPartAction
    {
        [Description("Keep At SC")]
        KeepAtSC,

        [Description("Return To EVM")]
        ReturnToEVM
    }

    public static class ExcessPartActionExtensions
    {
        public static string GetDescription(this ExcessPartAction action)
        {
            var memberInfo = typeof(ExcessPartAction).GetField(action.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? action.ToString();
        }

        public static List<string> GetAllActions()
        {
            return Enum.GetValues<ExcessPartAction>()
                .Select(a => a.GetDescription())
                .ToList();
        }
    }

    /// <summary>
    /// Hành ??ng x? lý cho ph? tùng thi?u
    /// </summary>
    public enum ShortagePartAction
    {
        [Description("Compensate")]
        Compensate,

        [Description("Reship")]
        Reship,

        [Description("Accept Loss")]
        AcceptLoss
    }

    public static class ShortagePartActionExtensions
    {
        public static string GetDescription(this ShortagePartAction action)
        {
            var memberInfo = typeof(ShortagePartAction).GetField(action.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? action.ToString();
        }

        public static List<string> GetAllActions()
        {
            return Enum.GetValues<ShortagePartAction>()
                .Select(a => a.GetDescription())
                .ToList();
        }
    }
}
