using System;
using System.Collections.Generic;

namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class WorkOrderDetailDto
    {
        public Guid? WorkOrderId { get; set; }
        public Guid? AssignedTo { get; set; }
        public string? Type { get; set; }
        public string? Target { get; set; }
        public Guid? TargetId { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Notes { get; set; }

        public WarrantyClaimInfoDto? WarrantyClaim { get; set; }
    }

    public class WarrantyClaimInfoDto
    {
        public Guid ClaimId { get; set; }
        public string? Vin { get; set; }
        public string? FailureDesc { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        // vehicle info
        public string? Model { get; set; }
        public int? Year { get; set; }

        public IEnumerable<ShowClaimPartDto>? ClaimParts { get; set; }
        public IEnumerable<ImageDto>? Attachments { get; set; }
    }
}
