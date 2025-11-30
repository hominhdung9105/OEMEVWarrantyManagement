//using System.ComponentModel;

//namespace OEMEVWarrantyManagement.Share.Enums
//{
//    public enum VehiclePartStatus
//    {
//        [Description("Installed")]
//        Installed,

//        [Description("UnInstalled")]
//        UnInstalled
//    }

//    public static class VehiclePartStatusExtensions
//    {
//        public static string GetVehiclePartStatus(this VehiclePartStatus status)
//        {
//            var memberInfo = typeof(VehiclePartStatus).GetField(status.ToString());
//            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
//            return attribute?.Description ?? status.ToString();
//        }
//    }
//}