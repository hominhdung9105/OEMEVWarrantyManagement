namespace OEMEVWarrantyManagement.Database.Models
{
    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<RoleEmployee> RoleEmployees { get; set; } = new List<RoleEmployee>();
    }
}
