using System.ComponentModel;
using System.Reflection;

namespace OEMEVWarrantyManagement.Share.Enums
{
    public enum TimeSlotEnum
    {
        [Description("08:00")]
        Slot1,
        [Description("09:00")]
        Slot2,
        [Description("10:00")]
        Slot3,
        [Description("11:00")]
        Slot4,
        [Description("14:00")]
        Slot5,
        [Description("15:00")]
        Slot6,
        [Description("16:00")]
        Slot7,
        [Description("17:00")]
        Slot8
    }

    public static class TimeSlotExtensions
    {
        // Lấy giờ từ Description
        public static string GetTime(this TimeSlotEnum slot)
        {
            var member = typeof(TimeSlotEnum).GetMember(slot.ToString()).FirstOrDefault();
            var attr = member?.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? string.Empty;
        }

        // Parse từ string (ví dụ "Slot3") sang enum
        public static TimeSlotEnum? FromString(string slotString)
        {
            if (Enum.TryParse<TimeSlotEnum>(slotString, ignoreCase: true, out var slot))
                return slot;
            return null;
        }

        // Lấy info tổng hợp
        public static object? GetSlotInfo(string slotString)
        {
            var slot = FromString(slotString);
            if (slot == null) return null;

            return new
            {
                slot = slot.Value.ToString(),
                time = slot.Value.GetTime()
            };
        }

        // Lấy tất cả các slot với thông tin chi tiết
        public static IEnumerable<dynamic> GetAllSlots()
        {
            return Enum.GetValues<TimeSlotEnum>()
           .Select(slot => new
           {
                slot = slot.ToString(),
                time = slot.GetTime()
            });
        }
    }
}
