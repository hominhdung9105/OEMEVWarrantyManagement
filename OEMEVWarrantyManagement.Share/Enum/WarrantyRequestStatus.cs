namespace OEMEVWarrantyManagement.Share.Enum
{
    public enum WarrantyRequestStatus
    {
        [StatusAttr("waiting for unassigned")]
        WaitingForUnassigned, // Chưa phân công kỹ thuật viên
        [StatusAttr("under inspection")]
        UnderInspection, // Kỹ thuật viên đang kiểm tra
        [StatusAttr("pending confirmation")]
        PendingConfirmation, // Chưa xác nhận kết quả
        [StatusAttr("sent to manufacturer")]
        SentToManufacturer, // Đã gửi về hãng
        [StatusAttr("approved")]
        Approved, // Được chấp nhận bảo hành
        [StatusAttr("denied")]
        Denied // Bị từ chối bảo hành
    }

    public static class WarrantyRequestStatusExtensions
    {
        public static string GetWarrantyRequestStatus(this WarrantyRequestStatus error)
        {
            var memberInfo = typeof(WarrantyRequestStatus).GetField(error.ToString());
            return ((StatusAttr)Attribute.GetCustomAttribute(memberInfo, typeof(StatusAttr))).Status;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class StatusAttr(string status) : Attribute
    {
        public string Status { get; } = status;
    }
}
