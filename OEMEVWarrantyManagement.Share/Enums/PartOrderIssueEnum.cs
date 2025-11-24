using System.ComponentModel;

namespace OEMEVWarrantyManagement.Share.Enums
{
    /// <summary>
    /// Lý do H?Y lô hàng (m?t h?t, không quay v?)
    /// </summary>
    public enum PartOrderCancellationReason
    {
        [Description("Tai n?n giao thông/H? h?ng toàn b?")]
        AccidentTotalDamage,

        [Description("M?t c?p/Th?t l?c hoàn toàn")]
        TheftOrLost,

        [Description("Cháy n?")]
        FireExplosion,

        [Description("Thiên tai")]
        NaturalDisaster,

        [Description("Lý do khác")]
        Other
    }

    /// <summary>
    /// Lý do TR? hàng v? (hàng quay v? kho EVM)
    /// </summary>
    public enum PartOrderReturnReason
    {
        [Description("Không giao ???c/T? ch?i nh?n hàng")]
        DeliveryRefused,

        [Description("??a ch? sai/Không liên l?c ???c")]
        WrongAddressUnreachable,

        [Description("Service Center t?m ?óng c?a")]
        ServiceCenterClosed,

        [Description("H?y ??n t? phía Service Center")]
        CancelledBySC,

        [Description("Lý do khác")]
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

        [Description("EVM Responsible")]
        EvmResponsible,

        [Description("SC Responsible")]
        ScResponsible,

        [Description("Shared Responsibility")]
        SharedResponsibility,

        [Description("Transport Responsible")]
        TransportResponsible,

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
}
