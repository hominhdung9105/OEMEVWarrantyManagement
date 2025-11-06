using System.ComponentModel;

namespace OEMEVWarrantyManagement.Share.Enums
{
    public enum AppointmentStatus
    {
        [Description("Pending")]
        Pending,
        [Description("Scheduled")]
        Scheduled,
        [Description("Checked_In")]
        CheckedIn,
        [Description("Cancelled")]
        Cancelled,
        [Description("Done")]
        Done,
        [Description("No_show")]
        NoShow
    }

    public static class AppointmentStatusExtensions
    {
        public static string GetAppointmentStatus(this AppointmentStatus status)
        {
            var memberInfo = typeof(AppointmentStatus).GetField(status.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? status.ToString();
        }
    }
}
