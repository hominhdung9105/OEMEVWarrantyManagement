using System.ComponentModel;

namespace OEMEVWarrantyManagement.Share.Enums
{
    public enum PartCategory
    {
        [Description("Battery")]
        Battery,

        [Description("dsfsd")]
        Repair
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
    }
}
