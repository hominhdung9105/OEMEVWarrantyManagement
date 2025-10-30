using System.ComponentModel;

namespace OEMEVWarrantyManagement.Share.Enums
{
    public enum CampaignVehicleStatus
    {
        [Description("waiting for unassigned repair")]
        WaitingForUnassignedRepair,

        [Description("under repair")]
        UnderRepair,

        [Description("repaired")]
        Repaired,

        [Description("done")]
        Done
    }

    public static class CampaignVehicleStatusExtensions
    {
        public static string GetCampaignVehicleStatus(this CampaignVehicleStatus status)
        {
            var memberInfo = typeof(CampaignVehicleStatus).GetField(status.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? status.ToString();
        }
    }
}
