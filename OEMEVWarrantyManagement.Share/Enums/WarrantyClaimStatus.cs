using System.ComponentModel;

namespace OEMEVWarrantyManagement.Share.Enums
{
    public enum WarrantyClaimStatus
    {
        [Description("waiting for unassigned")]
        WaitingForUnassigned, // Chưa phân công kỹ thuật viên

        [Description("under inspection")]
        UnderInspection, // Kỹ thuật viên đang kiểm tra

        [Description("pending confirmation")]
        PendingConfirmation, // Chưa xác nhận kết quả

        [Description("sent to manufacturer")]
        SentToManufacturer, // Đã gửi về hãng

        [Description("denied")]
        Denied, // Bị từ chối bảo hành

        [Description("approved")]
        Approved, // Được chấp nhận bảo hành

        [Description("waiting for unassigned repair")]
        WaitingForUnassignedRepair, // Chờ phân công kỹ thuật viên bảo hành

        [Description("under repair")]
        UnderRepair, // Kỹ thuật viên đang bảo hành

        [Description("repaired")]
        Repaired, // Bảo hành xong

        [Description("car back home")]
        CarBackHome, // Khách lấy xe về

        [Description("done warranty")]
        DoneWarranty // Giao xe cho khách xong
    }

    public static class WarrantyClaimStatusExtensions
    {
        public static string GetWarrantyClaimStatus(this WarrantyClaimStatus status)
        {
            var memberInfo = typeof(WarrantyClaimStatus).GetField(status.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? status.ToString();
        }

        public static List<string> GetAllStatus()
        {
            var statusList = new List<string>();
            foreach (var status in Enum.GetValues<WarrantyClaimStatus>())
            {
                statusList.Add(status.GetWarrantyClaimStatus());
            }
            return statusList;
        }
    }

    /// <summary>
    /// Lý do từ chối bảo hành
    /// </summary>
    public enum WarrantyClaimDenialReason
    {
        [Description("Warranty Expired")]
        WarrantyExpired,

        [Description("Not Covered Under Warranty")]
        NotCovered,

        [Description("Improper Use or Maintenance")]
        ImproperUse,

        [Description("Unauthorized Modifications")]
        UnauthorizedModifications,

        [Description("Accident or Collision Damage")]
        AccidentDamage,

        [Description("Missing Required Documentation")]
        MissingDocumentation,

        [Description("Other")]
        Other
    }

    public static class WarrantyClaimDenialReasonExtensions
    {
        public static string GetDenialReason(this WarrantyClaimDenialReason reason)
        {
            var memberInfo = typeof(WarrantyClaimDenialReason).GetField(reason.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? reason.ToString();
        }

        public static List<string> GetAllDenialReasons()
        {
            var reasons = new List<string>();
            foreach (var reason in Enum.GetValues<WarrantyClaimDenialReason>())
            {
                reasons.Add(reason.GetDenialReason());
            }
            return reasons;
        }

        /// <summary>
        /// Parse WarrantyClaimDenialReason from description string
        /// </summary>
        public static WarrantyClaimDenialReason? FromDescription(string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return null;

            foreach (WarrantyClaimDenialReason reason in Enum.GetValues(typeof(WarrantyClaimDenialReason)))
            {
                var memberInfo = typeof(WarrantyClaimDenialReason).GetField(reason.ToString());
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
                
                if (attribute?.Description != null && 
                    string.Equals(attribute.Description, description, StringComparison.OrdinalIgnoreCase))
                {
                    return reason;
                }
            }

            return null;
        }
    }
}
