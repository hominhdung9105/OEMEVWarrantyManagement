using System.ComponentModel;

namespace OEMEVWarrantyManagement.Share.Enums
{
    public enum PartCategory
    {
        [Description("Battery Pack")]
        BatteryPack,

        [Description("Battery Management System (BMS)")]
        BMS,

        [Description("Electric Motor")]
        ElectricMotor,

        [Description("Inverter / Controller")]
        Inverter,

        [Description("On-board Charger")]
        OnboardCharger,

        [Description("Charging Port / Connector")]
        ChargingPort,

        [Description("DC-DC Converter")]
        DCDCConverter,

        [Description("Cooling System")]
        CoolingSystem,

        [Description("Thermal Management")]
        ThermalManagement,

        [Description("High Voltage Cable / Harness")]
        HighVoltageCable,

        [Description("Low Voltage Electrical System")]
        LowVoltageElectrical,

        [Description("Brake System (Regenerative / Hydraulic)")]
        BrakeSystem,

        [Description("Suspension System")]
        Suspension,

        [Description("Steering System")]
        Steering,

        [Description("Body & Frame Components")]
        BodyFrame,

        [Description("Interior / Cabin Parts")]
        Interior,

        [Description("Lighting System")]
        Lighting,

        [Description("Sensor / ECU / Control Unit")]
        ControlUnit,

        [Description("Software / Firmware Module")]
        SoftwareModule,

        [Description("Tire / Wheel")]
        TireWheel,

        [Description("Charging Cable / Accessory")]
        ChargingAccessory,

        [Description("Other / Miscellaneous")]
        Other
    }


    public static class PartCategoryExtensions
    {
        public static string GetPartCategory(this PartCategory category)
        {
            var memberInfo = typeof(PartCategory).GetField(category.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? category.ToString();
        }

        public static List<string> GetAllCategories()
        {
            List<string> categories = new List<string>();
            foreach (var category in Enum.GetValues<PartCategory>())
            {
                categories.Add(((PartCategory)category).GetPartCategory());
            }
            return categories;
        }

        public static bool IsValidCategory(string category)
        {
            var listCategory = GetAllCategories();
            return listCategory.Contains(category);
        }
    }

    public static class PartModel
    {
        // Tạo mapping giữa category và danh sách model
        public static readonly Dictionary<string, List<string>> ModelsByCategory =
    new Dictionary<string, List<string>>
    {
        {
            PartCategory.BatteryPack.GetPartCategory(), new List<string>
            {
                "EV-LFP-72V-40Ah",
                "EV-NCM-350V-60Ah",
                "EV-NCA-400V-80Ah"
            }
        },
        {
            PartCategory.BMS.GetPartCategory(), new List<string>
            {
                "BMS Smart V1",
                "BMS Smart V2",
                "BMS Lite 48V"
            }
        },
        {
            PartCategory.ElectricMotor.GetPartCategory(), new List<string>
            {
                "Motor 3kW Hub",
                "Motor 5kW Mid Drive",
                "Motor 10kW PMSM"
            }
        },
        {
            PartCategory.Inverter.GetPartCategory(), new List<string>
            {
                "Inverter 48V-3kW",
                "Inverter 350V-10kW"
            }
        },
        {
            PartCategory.OnboardCharger.GetPartCategory(), new List<string>
            {
                "OBC 1.5kW",
                "OBC 3.3kW",
                "OBC 6.6kW"
            }
        },
        {
            PartCategory.ChargingPort.GetPartCategory(), new List<string>
            {
                "Type 2 Connector",
                "GB/T Port",
                "CCS Combo"
            }
        },
        {
            PartCategory.DCDCConverter.GetPartCategory(), new List<string>
            {
                "DC-DC 48V/12V-500W",
                "DC-DC 400V/12V-1kW"
            }
        },
        {
            PartCategory.CoolingSystem.GetPartCategory(), new List<string>
            {
                "Liquid Cooling Pump",
                "Radiator Assembly",
                "Coolant Reservoir"
            }
        },
        {
            PartCategory.BrakeSystem.GetPartCategory(), new List<string>
            {
                "Disc Brake Set",
                "Regenerative Brake Controller"
            }
        },
        {
            PartCategory.Suspension.GetPartCategory(), new List<string>
            {
                "Front Fork Assembly",
                "Rear Shock Absorber"
            }
        },
        {
            PartCategory.Steering.GetPartCategory(), new List<string>
            {
                "Handlebar Assembly",
                "Steering Column"
            }
        },
        {
            PartCategory.BodyFrame.GetPartCategory(), new List<string>
            {
                "Aluminum Frame",
                "Plastic Body Cover Set"
            }
        },
        {
            PartCategory.Lighting.GetPartCategory(), new List<string>
            {
                "LED Headlight",
                "LED Taillight",
                "Turn Signal Set"
            }
        },
        {
            PartCategory.ControlUnit.GetPartCategory(), new List<string>
            {
                "Main ECU V1",
                "Motor Controller MCU",
                "Display Unit"
            }
        },
        {
            PartCategory.SoftwareModule.GetPartCategory(), new List<string>
            {
                "Firmware v1.0",
                "Battery Calibration Tool",
                "Diagnostics App"
            }
        },
        {
            PartCategory.TireWheel.GetPartCategory(), new List<string>
            {
                "Tire 16 inch",
                "Tire 18 inch",
                "All-terrain Tire"
            }
        },
        {
            PartCategory.ChargingAccessory.GetPartCategory(), new List<string>
            {
                "Portable Charger",
                "Charging Cable 5m",
                "Wallbox Charger"
            }
        },
        {
            PartCategory.Other.GetPartCategory(), new List<string>
            {
                "Seat Cushion",
                "Footrest",
                "Mirror Set"
            }
        }
    };


        public static List<string> GetModels(string category)
        {
            return ModelsByCategory.TryGetValue(category, out var models)
                ? models
                : new List<string>(); // nếu không có thì trả list rỗng
        }

        // Check if a model exists across all categories (case-insensitive)
        public static bool IsValidModel(string model)
        {
            if (string.IsNullOrWhiteSpace(model)) return false;

            return ModelsByCategory.Values
                .SelectMany(list => list)
                .Any(m => string.Equals(m, model, StringComparison.OrdinalIgnoreCase));
        }
    }
}
