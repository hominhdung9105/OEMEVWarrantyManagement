using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class ClaimPartDto
    {
        public Guid ClaimPartId { get; set; }
        public Guid ClaimId { get; set; }
        public Guid PartId { get; set; }
        public string Model { get; set; }
        public int Quantity { get; set; }
        public string Action { get; set; } // repair | replace
        public string Status { get; set; }
        public decimal Cost { get; set; }
    }

    public class RequestClaimPart
    {
        public Guid ClaimId { get; set; }
        public Guid PartId { get; set; }
        public string Model { get; set; }
        public int Quantity { get; set; }
        public string Action { get; set; }
        public string? Status { get; set; }
    }

    public class PartsInClaimPartDto
    {
        public Guid PartId { get; set; }
        public string Model { get; set; }
        public int Quantity { get; set; }
        public string Action { get; set; }
        public string? Status { get; set; }
    }

    public class CreateClaimPartsRequest
    {
        public Guid? ClaimId { get; set; }
        public List<PartsInClaimPartDto> Parts { get; set; }
    }
}
