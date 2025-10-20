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

        [Description("hold customer car")]
        HoldCustomerCar, // Giữ xe khách để bảo hành

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
            List<string> statusList = [];
            foreach (var status in Enum.GetValues<WarrantyClaimStatus>())
            {
                statusList.Add(((WarrantyClaimStatus)status).GetWarrantyClaimStatus());
            }
            return statusList;
        }
    }
}
