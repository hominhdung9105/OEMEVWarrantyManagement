using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Employee
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public int OrgId { get; set; }

        // Navigation Properties
        public Organization Organization { get; set; }
        public ICollection<WarrantyClaim> CreatedClaims { get; set; } = new List<WarrantyClaim>();
        public ICollection<WarrantyClaim> ApprovedClaims { get; set; } = new List<WarrantyClaim>();
        public ICollection<ClaimAttachment> UploadedAttachments { get; set; } = new List<ClaimAttachment>();
        public ICollection<WorkOrder> AssignedWorkOrders { get; set; } = new List<WorkOrder>();
        public ICollection<PartOrder> CreatedPartOrders { get; set; } = new List<PartOrder>();
    }
}
