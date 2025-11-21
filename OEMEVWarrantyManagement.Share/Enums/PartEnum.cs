using System.ComponentModel;

namespace OEMEVWarrantyManagement.Share.Enums
{
    public enum PartCategory
    {
        // 1. BODY & EXTERIOR
        [Description("Body & Exterior")]
        BodyExterior,

        // 2. AUTOMOTIVE LIGHTING SYSTEMS
        [Description("Lighting")]
        Lighting,

        // 3. CHASSIS, SUSPENSION & STEERING
        [Description("Suspension")]
        Suspension,

        [Description("Steering")]
        Steering,

        [Description("Wheels")]
        Wheels,

        // 4. POWERTRAIN – EV (giữ lại)
        [Description("EV Powertrain")]
        EVPowertrain,

        // 6. BRAKING SYSTEMS
        [Description("Brakes")]
        Brakes,

        // 7. ELECTRICAL & ELECTRONICS
        [Description("ADAS")]
        ADAS,

        [Description("Safety Electronics")]
        SafetyElectronics,

        // 8. INTERIOR & HMI
        [Description("Infotainment")]
        Infotainment,

        [Description("Seating")]
        Seating,

        [Description("HVAC")]
        HVAC
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
        // Mapping giữa PartCategory và danh sách Parts (mẫu / model)
        public static readonly Dictionary<string, List<string>> ModelsByCategory =
            new Dictionary<string, List<string>>
        {
    // 1. BODY & EXTERIOR
    {
        PartCategory.BodyExterior.GetPartCategory(), new List<string>
        {
            "Front Bumper",
            "Rear Bumper",
            "Hood / Bonnet",
            "Front Fenders",
            "Rear Fenders",
            "Roof Panel",
            "Doors",
            "Door Handles",
            "Side Mirrors",
            "Windows / Glass",
            "Panoramic Sunroof",
            "Exterior Trim / Moldings"
        }
    },

    // 2. LIGHTING
    {
        PartCategory.Lighting.GetPartCategory(), new List<string>
        {
            "LED Headlamps",
            "LED Tail Lamps",
            "Fog Lights",
            "Daytime Running Lights (DRL)",
            "Turn Signal Lights",
            "License Plate Lights",
            "Interior Lighting"
        }
    },

    // 3. SUSPENSION
    {
        PartCategory.Suspension.GetPartCategory(), new List<string>
        {
            "Front Suspension (MacPherson / Double Wishbone)",
            "Rear Suspension (Multi-link / Torsion Beam)",
            "Shock Absorbers / Dampers",
            "Coil Springs / Air Springs",
            "Control Arms",
            "Stabilizer Bars / Sway Bars",
            "Suspension Bushings / Mounts"
        }
    },

    // 3. STEERING
    {
        PartCategory.Steering.GetPartCategory(), new List<string>
        {
            "Electric Power Steering (EPS) Motor",
            "Steering Column",
            "Steering Rack / Gear",
            "Tie Rods / Ends",
            "Steering Knuckles",
            "Steering Wheel Controls"
        }
    },

    // 3. WHEELS
    {
        PartCategory.Wheels.GetPartCategory(), new List<string>
        {
            "Front Wheels / Rims",
            "Rear Wheels / Rims",
            "Tires (All-season / Performance)",
            "Wheel Nuts / Bolts",
            "TPMS Sensors"
        }
    },

    // 4. EV POWERTRAIN
    {
        PartCategory.EVPowertrain.GetPartCategory(), new List<string>
        {
            "Single Motor (FWD)",
            "Dual Motor (AWD)",
            "Motor Controller / Inverter",
            "Reduction Gear / Gearbox",
            "Driveshafts / Half-shafts",
            "AWD Coupling / Differential",
            "High-Voltage Battery Pack",
            "Battery Management System (BMS)",
            "Battery Cooling System",
            "HV Fuses / Disconnects",
            "Battery Mounting Brackets",
            "Onboard Charger (OBC)",
            "CCS Fast Charge Port",
            "AC Level 2 Charge Port",
            "Charge Cable / Plug",
            "Charge Controller / EVSE Interface"
        }
    },

    // 6. BRAKES
    {
        PartCategory.Brakes.GetPartCategory(), new List<string>
        {
            "Brake Calipers",
            "Brake Discs / Rotors",
            "Brake Pads / Shoes",
            "Brake Lines / Hoses",
            "Brake Master Cylinder",
            "Brake Booster / Servo",
            "Regenerative Braking System",
            "Parking Brake / Electronic Handbrake"
        }
    },

    // 7. ADAS
    {
        PartCategory.ADAS.GetPartCategory(), new List<string>
        {
            "Adaptive Cruise Control",
            "Lane Keeping Camera",
            "Surround / 360º Cameras",
            "Parking Sensors",
            "Highway Assist Module",
            "Summon / Autonomous Parking Module"
        }
    },

    // 7. SAFETY ELECTRONICS
    {
        PartCategory.SafetyElectronics.GetPartCategory(), new List<string>
        {
            "Airbags (Front / Side / Curtain)",
            "Seatbelt Pre-tensioners",
            "12V Battery",
            "DC-DC Converter",
            "Fuses / Relays / Circuit Breakers",
            "Control Modules (ECU / BCM)",
            "LV Sensors (Temperature / Pressure)",
            "HV Harness & Wiring",
            "LV Harness & Wiring"
        }
    },

    // 8. INFOTAINMENT
    {
        PartCategory.Infotainment.GetPartCategory(), new List<string>
        {
            "Central Touchscreen",
            "Head-Up Display (HUD)",
            "Audio System (Speakers / Amplifier)",
            "GPS / Navigation Module",
            "Wi-Fi / LTE Modem",
            "USB Ports / Wireless Charging",
            "Smartphone Integration (CarPlay / Android Auto)",
            "Voice Assistant Microphone",
            "Karaoke System"
        }
    },

    // 8. SEATING
    {
        PartCategory.Seating.GetPartCategory(), new List<string>
        {
            "Front Seats",
            "Rear Seats",
            "Seat Belts",
            "Headrests",
            "Floor Mats / Carpeting",
            "Storage Compartments",
            "Cup Holders"
        }
    },

    // 8. HVAC
    {
        PartCategory.HVAC.GetPartCategory(), new List<string>
        {
            "HVAC Vents",
            "HVAC Blower Motor",
            "Heater Core",
            "Thermal Sensors / Thermostats",
            "Battery Coolant Pump",
            "Radiator / Heat Exchanger",
            "HVAC Compressor",
            "Cooling Hoses & Piping"
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

        // Get category by model name
        public static string? GetCategoryByModel(string model)
        {
            if (string.IsNullOrWhiteSpace(model)) return null;

            var category = ModelsByCategory
                .FirstOrDefault(kvp => kvp.Value.Any(m => string.Equals(m, model, StringComparison.OrdinalIgnoreCase)))
                .Key;

            return category;
        }

    }
    public enum PartStatus
    {
        [Description("In Stock")]
        InStock,
        [Description("Out of Stock")]
        OutOfStock,
        [Description("Low Stock")]
        LowStock
    }

    public static class PartStatusExtensions
    {
        public static string GetPartStatus(this PartStatus status)
        {
            var memberInfo = typeof(PartStatus).GetField(status.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute));
            return attribute?.Description ?? status.ToString();
        }
    }
}