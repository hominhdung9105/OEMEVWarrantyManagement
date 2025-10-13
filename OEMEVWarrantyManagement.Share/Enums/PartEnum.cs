using System;
using System.Collections.Generic;
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
