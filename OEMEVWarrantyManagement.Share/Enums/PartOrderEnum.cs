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
        Done,

        [Description("Cancelled")]
        Cancelled,

        [Description("Returning")]
        Returning,

        [Description("Return Inspection")]
        ReturnInspection,

        [Description("Discrepancy Review")]
        DiscrepancyReview
    }

    public static class PartOrderStatusExtensions
    {
        public static string GetPartOrderStatus(this PartOrderStatus status)
        {
            var memberInfo = typeof(PartOrderStatus).GetField(status.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? status.ToString();
        }

        /// <summary>
        /// Parse PartOrderStatus from description string (e.g., "Pending", "In Transit")
        /// </summary>
        public static PartOrderStatus? FromDescription(string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return null;

            foreach (PartOrderStatus status in Enum.GetValues(typeof(PartOrderStatus)))
            {
                var memberInfo = typeof(PartOrderStatus).GetField(status.ToString());
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
                
                if (attribute?.Description != null && 
                    string.Equals(attribute.Description, description, StringComparison.OrdinalIgnoreCase))
                {
                    return status;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Trạng thái của serial trong shipment (hàng gửi đi)
    /// </summary>
    public enum PartOrderShipmentStatus
    {
        [Description("Pending")]
        Pending,

        [Description("Confirmed")]
        Confirmed,

        [Description("Damaged")]
        Damaged,

        [Description("Missing")]
        Missing
    }

    public static class PartOrderShipmentStatusExtensions
    {
        public static string GetStatus(this PartOrderShipmentStatus status)
        {
            var memberInfo = typeof(PartOrderShipmentStatus).GetField(status.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? status.ToString();
        }
    }

    /// <summary>
    /// Trạng thái của serial trong receipt (hàng nhận được)
    /// </summary>
    public enum PartOrderReceiptStatus
    {
        [Description("Received")]
        Received,

        [Description("Damaged")]
        Damaged,

        [Description("Missing")]
        Missing
    }

    public static class PartOrderReceiptStatusExtensions
    {
        public static string GetStatus(this PartOrderReceiptStatus status)
        {
            var memberInfo = typeof(PartOrderReceiptStatus).GetField(status.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? status.ToString();
        }
    }
}
