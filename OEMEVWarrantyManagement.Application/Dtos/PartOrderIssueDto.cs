namespace OEMEVWarrantyManagement.Application.Dtos
{
    /// <summary>
    /// DTO ?? admin h?y lô hàng (m?t h?t, không quay v?)
    /// </summary>
    public class CancelShipmentRequestDto
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; } // T? enum PartOrderCancellationReason
        public string? ReasonDetail { get; set; } // B?t bu?c n?u Reason = "Other"
        public string? Note { get; set; }
    }

    /// <summary>
    /// DTO ?? EVM/Admin báo hàng tr? v? (hàng quay v? kho EVM)
    /// </summary>
    public class ReturnShipmentRequestDto
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; } // T? enum PartOrderReturnReason
        public string? ReasonDetail { get; set; } // B?t bu?c n?u Reason = "Other"
        public string? Note { get; set; }
    }

    /// <summary>
    /// DTO ?? EVM t?o ??n hàng m?i
    /// </summary>
    public class CreatePartOrderByEvmRequestDto
    {
        public Guid ServiceCenterId { get; set; }
        public List<PartOrderItemRequestDto> Items { get; set; } = new List<PartOrderItemRequestDto>();
    }

    public class PartOrderItemRequestDto
    {
        public string Model { get; set; }
        public int Quantity { get; set; }
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// DTO ?? admin quy?t ??nh v? sai l?ch
    /// </summary>
    public class ResolveDiscrepancyRequestDto
    {
        public Guid OrderId { get; set; }
        public string ResponsibleParty { get; set; } // "EVM", "SC", "Transport", "Shared"
        public string Decision { get; set; }
        public string? Note { get; set; }
    }

    /// <summary>
    /// Response DTO cho danh sách lý do h?y
    /// </summary>
    public class CancellationReasonDto
    {
        public string Value { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Response DTO cho danh sách lý do tr? hàng
    /// </summary>
    public class ReturnReasonDto
    {
        public string Value { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// DTO cho thông tin v?n ?? ??n hàng
    /// </summary>
    public class PartOrderIssueDto
    {
        public Guid IssueId { get; set; }
        public Guid OrderId { get; set; }
        public string IssueType { get; set; }
        public string Reason { get; set; }
        public string? ReasonDetail { get; set; }
        public string? Note { get; set; }
        public Guid CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO cho quy?t ??nh sai l?ch
    /// </summary>
    public class DiscrepancyResolutionDto
    {
        public Guid ResolutionId { get; set; }
        public Guid OrderId { get; set; }
        public string Status { get; set; }
        public string? ResponsibleParty { get; set; }
        public string? Decision { get; set; }
        public string? Note { get; set; }
        public Guid? ResolvedBy { get; set; }
        public string? ResolvedByName { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
