namespace OEMEVWarrantyManagement.Share.Enum
{
    public enum WarrantyClaimStatus
    {
        [StatusAttr("waiting for unassigned")]
        WaitingForUnassigned, // Chưa phân công kỹ thuật viên

        [StatusAttr("under inspection")]
        UnderInspection, // Kỹ thuật viên đang kiểm tra
        [StatusAttr("pending confirmation")]
        PendingConfirmation, // Chưa xác nhận kết quả

        [StatusAttr("sent to manufacturer")]
        SentToManufacturer, // Đã gửi về hãng
        [StatusAttr("denied")]
        Denied, // Bị từ chối bảo hành

        [StatusAttr("approved")]
        Approved, // Được chấp nhận bảo hành

        [StatusAttr("waiting for repair")]
        WaitingForRepair, // Chờ để tech tiến hành bảo hành

        [StatusAttr("under repair")]
        UnderRepair, // Kỹ thuật viên đang bảo hành

        [StatusAttr("repaired")]
        Repaired, //Bao hanh xong 

        [StatusAttr("done warranty")]
        DoneWarranty //Giao xe cho khách xong

    }

    public static class WarrantyRequestStatusExtensions
    {
        public static string GetWarrantyRequestStatus(this WarrantyClaimStatus error)
        {
            var memberInfo = typeof(WarrantyClaimStatus).GetField(error.ToString());
            return ((StatusAttr)Attribute.GetCustomAttribute(memberInfo, typeof(StatusAttr))).Status;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class StatusAttr(string status) : Attribute
    {
        public string Status { get; } = status;
    }
}
