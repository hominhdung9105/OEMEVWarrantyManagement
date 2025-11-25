using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.Application.Dtos
{
    /// <summary>
    /// DTO ?? admin h?y lô hàng (m?t h?t, không quay v?)
    /// </summary>
    public class CancelShipmentRequestDto
    {
        public Guid OrderId { get; set; }
        public PartOrderCancellationReason Reason { get; set; } // T? enum PartOrderCancellationReason
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
    /// DTO ?? admin quy?t ??nh v? sai l?ch - m?i: x? lý t?ng ph? tùng
    /// </summary>
    public class ResolveDiscrepancyRequestDto
    {
        public Guid OrderId { get; set; }
        
        /// <summary>
        /// Danh sách quy?t ??nh cho t?ng ph? tùng b? sai l?ch (h? h?ng, d?, thi?u)
        /// </summary>
        public List<PartDiscrepancyResolutionDto> PartResolutions { get; set; } = new List<PartDiscrepancyResolutionDto>();
        
        /// <summary>
        /// Ghi chú chung cho toàn b? ??n hàng
        /// </summary>
        public string? OverallNote { get; set; }
    }

    /// <summary>
    /// Quy?t ??nh x? lý cho m?t ph? tùng c? th?
    /// </summary>
    public class PartDiscrepancyResolutionDto
    {
        /// <summary>
        /// Serial number c?a ph? tùng
        /// </summary>
        public string SerialNumber { get; set; }
        
        /// <summary>
        /// Model c?a ph? tùng
        /// </summary>
        public string Model { get; set; }
        
        /// <summary>
        /// Lo?i sai l?ch: "Damaged", "Excess", "Shortage"
        /// </summary>
        public string DiscrepancyType { get; set; }
        
        /// <summary>
        /// Bên ch?u trách nhi?m: "EVM", "SC", "Transport", "Shared"
        /// </summary>
        public string ResponsibleParty { get; set; }
        
        /// <summary>
        /// Hành ??ng x? lý c? th?
        /// Damaged: "Compensate", "Repair", "Accept_As_Is", "Return_To_EVM"
        /// Excess: "Keep_At_SC", "Return_To_EVM"
        /// Shortage: "Compensate", "Reship", "Accept_Loss"
        /// </summary>
        public string Action { get; set; }
        
        /// <summary>
        /// Ghi chú cho ph? tùng này
        /// </summary>
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
    /// DTO cho quy?t ??nh sai l?ch - phiên b?n m?i v?i chi ti?t t?ng ph? tùng
    /// </summary>
    public class DiscrepancyResolutionDto
    {
        public Guid ResolutionId { get; set; }
        public Guid OrderId { get; set; }
        public string Status { get; set; }
        public Guid? ResolvedBy { get; set; }
        public string? ResolvedByName { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Ghi chú chung cho toàn b? ??n hàng
        /// </summary>
        public string? OverallNote { get; set; }
        
        /// <summary>
        /// Danh sách quy?t ??nh cho t?ng ph? tùng
        /// </summary>
        public List<PartDiscrepancyDetailDto>? PartResolutions { get; set; }
    }

    /// <summary>
    /// Chi ti?t quy?t ??nh cho m?t ph? tùng
    /// </summary>
    public class PartDiscrepancyDetailDto
    {
        public Guid DetailId { get; set; }
        public Guid ResolutionId { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public string DiscrepancyType { get; set; } // Damaged, Excess, Shortage
        public string ResponsibleParty { get; set; } // EVM, SC, Transport, Shared
        public string Action { get; set; }
        public string? Note { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách các l?a ch?n v? lo?i sai l?ch
    /// </summary>
    public class DiscrepancyTypeOptionDto
    {
        public string Value { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách các bên ch?u trách nhi?m
    /// </summary>
    public class ResponsiblePartyOptionDto
    {
        public string Value { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách hành ??ng x? lý ph? tùng h? h?ng
    /// </summary>
    public class DamagedPartActionOptionDto
    {
        public string Value { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách hành ??ng x? lý ph? tùng d?
    /// </summary>
    public class ExcessPartActionOptionDto
    {
        public string Value { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách hành ??ng x? lý ph? tùng thi?u
    /// </summary>
    public class ShortagePartActionOptionDto
    {
        public string Value { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// DTO t?ng h?p t?t c? các l?a ch?n cho vi?c x? lý sai l?ch
    /// </summary>
    public class DiscrepancyResolutionOptionsDto
    {
        /// <summary>
        /// Danh sách lo?i sai l?ch
        /// </summary>
        public List<DiscrepancyTypeOptionDto> DiscrepancyTypes { get; set; } = new List<DiscrepancyTypeOptionDto>();
        
        /// <summary>
        /// Danh sách bên ch?u trách nhi?m
        /// </summary>
        public List<ResponsiblePartyOptionDto> ResponsibleParties { get; set; } = new List<ResponsiblePartyOptionDto>();
        
        /// <summary>
        /// Danh sách hành ??ng cho ph? tùng h? h?ng
        /// </summary>
        public List<DamagedPartActionOptionDto> DamagedPartActions { get; set; } = new List<DamagedPartActionOptionDto>();
        
        /// <summary>
        /// Danh sách hành ??ng cho ph? tùng d?
        /// </summary>
        public List<ExcessPartActionOptionDto> ExcessPartActions { get; set; } = new List<ExcessPartActionOptionDto>();
        
        /// <summary>
        /// Danh sách hành ??ng cho ph? tùng thi?u
        /// </summary>
        public List<ShortagePartActionOptionDto> ShortagePartActions { get; set; } = new List<ShortagePartActionOptionDto>();
    }
}
