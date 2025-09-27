using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class ClaimPart
    {
        public int ClaimPartId { get; set; }
        public int ClaimId { get; set; }
        public int PartId { get; set; }
        public string SerialNumber { get; set; }
        public string Action { get; set; } // repair | replace
        public decimal Cost { get; set; }

        // Navigation Properties
        public WarrantyClaim WarrantyClaim { get; set; }
        public Part Part { get; set; }
    }
}
