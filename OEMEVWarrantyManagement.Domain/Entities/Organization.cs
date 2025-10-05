using System.Text.Json.Serialization;


namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Organization
    {
        public Guid OrgId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // EVM | SC
        public string Region { get; set; }
        public string ContactInfo { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        [JsonIgnore]
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
        [JsonIgnore]
        public ICollection<Part> Parts { get; set; } = new List<Part>();
        [JsonIgnore]
        public ICollection<WarrantyPolicy> WarrantyPolicies { get; set; } = new List<WarrantyPolicy>();
        [JsonIgnore]
        public ICollection<WarrantyClaim> ServicedWarrantyClaims { get; set; } = new List<WarrantyClaim>();
        [JsonIgnore]
        public ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
        [JsonIgnore]
        public ICollection<PartOrder> PartOrders { get; set; } = new List<PartOrder>();
    }
}
