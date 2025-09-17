namespace OEMEVWarrantyManagement.Database.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<RoleEmployee> RoleEmployees { get; set; } = new List<RoleEmployee>();
    }
}
