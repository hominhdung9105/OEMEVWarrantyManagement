using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Organization
    {
        public int OrgId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // EVM | SC
        public string Region { get; set; }
        public string ContactInfo { get; set; }

        // Navigation Properties
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public ICollection<Part> Parts { get; set; } = new List<Part>();
        public ICollection<WarrantyPolicy> WarrantyPolicies { get; set; } = new List<WarrantyPolicy>();
        public ICollection<WarrantyClaim> ServicedWarrantyClaims { get; set; } = new List<WarrantyClaim>();
        public ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
        public ICollection<PartOrder> PartOrders { get; set; } = new List<PartOrder>();
    }
}
