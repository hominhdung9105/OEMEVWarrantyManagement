using System.ComponentModel;

namespace OEMEVWarrantyManagement.Share.Enums
{
    public enum CampaignStatus
    {
        [Description("Active")]
        Active,

        [Description("Closed")]
        Closed
    }

    public static class CampaignStatusExtensions
    {
        public static string GetCampaignStatus(this CampaignStatus status)
        {
            var memberInfo = typeof(CampaignStatus).GetField(status.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? status.ToString();
        }
    }

    public enum CampaignType
    {
        [Description("Recall")]
        Recall,
        [Description("Service")]
        Service
    }

    public static class CampaignTypeExtensions
    {
        public static string GetCampaignType(this CampaignType type)
        {
            var memberInfo = typeof(CampaignType).GetField(type.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? type.ToString();
        }

        public static bool IsValidType(string? type)
        {
            foreach (var enumValue in Enum.GetValues(typeof(CampaignType)))
            {
                if (string.Equals(enumValue.ToString(), type, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
