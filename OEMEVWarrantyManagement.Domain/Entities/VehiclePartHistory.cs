using System.Text.Json.Serialization;
using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class VehiclePartHistory
    {
        public Guid VehiclePartHistoryId { get; set; }
        public string Vin { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public DateTime InstalledAt { get; set; } // thời điểm gắn hoặc sự kiện ghi nhận
        public DateTime UninstalledAt { get; set; } // thời điểm tháo gỡ (nếu có)
        public DateTime ProductionDate { get; set; } // ngày xuất xưởng
        public int WarrantyPeriodMonths { get; set; } // thời gian bảo hành (tháng)
        public DateTime WarrantyEndDate { get; set; } // ngày hết hạn bảo hành
        public Guid ServiceCenterId { get; set; } // trung tâm hiện tại
        public string Condition { get; set; } // New | Used | Defective | Refurbished
        public string Status { get; set; } // InStock | OnVehicle | UnderRepair | Returned
        public string? Note { get; set; }

        [JsonIgnore]
        public Vehicle Vehicle { get; set; }
        [JsonIgnore]
        public Organization ServiceCenter { get; set; }
    }
}