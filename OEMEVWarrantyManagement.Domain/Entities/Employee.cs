using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class Employee
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public Guid OrgId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Organization Organization { get; set; }
        [JsonIgnore]
        public ICollection<WarrantyClaim> CreatedClaims { get; set; } = new List<WarrantyClaim>();
        [JsonIgnore]
        public ICollection<WarrantyClaim> ApprovedClaims { get; set; } = new List<WarrantyClaim>();
        [JsonIgnore]
        public ICollection<ClaimAttachment> UploadedAttachments { get; set; } = new List<ClaimAttachment>();
        [JsonIgnore]
        public ICollection<WorkOrder> AssignedWorkOrders { get; set; } = new List<WorkOrder>();
        [JsonIgnore]
        public ICollection<PartOrder> CreatedPartOrders { get; set; } = new List<PartOrder>();
    }
}
