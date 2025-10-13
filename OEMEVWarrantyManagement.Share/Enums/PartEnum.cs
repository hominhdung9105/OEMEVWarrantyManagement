using System.ComponentModel;

namespace OEMEVWarrantyManagement.Share.Enums
{
    public enum PartCategory
    {
        [Description("Battery")]
        Battery,

        [Description("Engine")]
        Engine,

        [Description("Tire")]
        Tire,

        [Description("dsfsd")]
        afaaf // TODO : Add more part categories as needed
    }

    public static class PartCategoryExtensions
    {
        public static string GetPartCategory(this PartCategory category)
        {
            var memberInfo = typeof(PartCategory).GetField(category.ToString());
            return ((DescriptionAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DescriptionAttribute))).ToString();
        }

        public static List<string> GetAllCategories()
        {
            List<string> categories = [];
            foreach (var category in Enum.GetValues<PartCategory>())
            {
                categories.Add(((PartCategory)category).GetPartCategory());
            }
            return categories;
        }

        public static bool IsValidCategory(string caterogry)
        {
            var listCategory = GetAllCategories();
            return listCategory.Contains(caterogry);
        }
    }

    public static class PartModel
    {
        // Tạo mapping giữa category và danh sách model
        public static readonly Dictionary<string, List<string>> ModelsByCategory =
            new Dictionary<string, List<string>>
            {
            {
                PartCategory.Battery.GetPartCategory(), new List<string>
                {
                    "Battery Model A",
                    "Battery Model B",
                    "Battery Model C"
                }
            },
            {
                PartCategory.Engine.GetPartCategory(), new List<string>
                {
                    "Engine V6",
                    "Engine V8",
                    "Engine Turbo"
                }
            },
            {
                PartCategory.Tire.GetPartCategory(), new List<string>
                {
                    "Tire 16 inch",
                    "Tire 18 inch"
                }
            }
            };

        public static List<string> GetModels(string category)
        {
            return ModelsByCategory.TryGetValue(category, out var models)
                ? models
                : new List<string>(); // nếu không có thì trả list rỗng
        }
    }
}
