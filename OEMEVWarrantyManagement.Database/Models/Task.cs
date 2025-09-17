namespace OEMEVWarrantyManagement.Database.Models
{
    public class Task
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
