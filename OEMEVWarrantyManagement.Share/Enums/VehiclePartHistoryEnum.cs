using System.ComponentModel;

namespace OEMEVWarrantyManagement.Share.Enums
{
    public enum VehiclePartCondition
    {
        [Description("New")] New,
        [Description("Used")] Used,
        [Description("Defective")] Defective, //Loi
        [Description("Refurbished")] Refurbished //Da duoc sua chua
    }

    public static class VehiclePartConditionExtensions
    {
        public static string GetCondition(this VehiclePartCondition condition)
        {
            var memberInfo = typeof(VehiclePartCondition).GetField(condition.ToString());
            var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(memberInfo!, typeof(DescriptionAttribute));
            return attribute?.Description ?? condition.ToString();
        }
    }

    public enum VehiclePartCurrentStatus
    {
        [Description("InStock")] InStock,
        [Description("OnVehicle")] OnVehicle,
        //[Description("UnderRepair")] UnderRepair,
        [Description("Returned")] Returned //Thuu hoi
    }

    public static class VehiclePartCurrentStatusExtensions
    {
        public static string GetCurrentStatus(this VehiclePartCurrentStatus status)
        {
            var memberInfo = typeof(VehiclePartCurrentStatus).GetField(status.ToString());
            var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(memberInfo!, typeof(DescriptionAttribute));
            return attribute?.Description ?? status.ToString();
        }
    }
}