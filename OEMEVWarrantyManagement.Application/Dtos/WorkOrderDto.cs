using System;

namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class WorkOrderDto
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

        // Vehicle information - added so inspection/repair responses include vehicle + customer info
        public string? Vin { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhoneNumber { get; set; }

        // Enriched info similar to WorkOrderDetailDto
        public WarrantyClaimInfoDto? WarrantyClaim { get; set; }
        public CampaignInfoDto? Campaign { get; set; }
    }
    public class RequestCreateWorkOrderDto
    {
        public Guid? WorkOrderId { get; set; }
        public Guid? AssignedTo { get; set; }
        public string? Type { get; set; } // kiểm tra/ sửa chữa / thay thế
        public string? Target { get; set; }// claim hay campaign
        public Guid? TargetId { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
    }

    public class RequestCreateWorkOrdersDto
    {
        public List<string> AssignedTo { get; set; }
        public string? Target { get; set; }// claim hay campaign
        public Guid? TargetId { get; set; }
    }

    public class RequestCreateAndUpdateDto
    {
        public Guid? ClaimId { get; set; }
        public Guid? AssignedTo { get; set; }
        public string? Type { get; set; } // kiểm tra/ sửa chữa / thay thế
        public string? Target { get; set; }// claim hay campaign
        public Guid? TargetId { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
    }
}