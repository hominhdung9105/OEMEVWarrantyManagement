namespace OEMEVWarrantyManagement.Application.Dtos
{
    /// <summary>
    /// Response DTO chi ti?t ??y ?? cho Part Order
    /// </summary>
    public class PartOrderDetailDto
    {
        public Guid OrderId { get; set; }
        public Guid ServiceCenterId { get; set; }
        public string ServiceCenterName { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string Status { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateOnly? ExpectedDate { get; set; }
        public DateTime? PartDelivery { get; set; }
        
        // Order Items
        public List<PartOrderItemDetailDto> Items { get; set; } = new List<PartOrderItemDetailDto>();
        
        // Shipment Info (n?u ?ã g?i)
        public ShipmentInfoDto? ShipmentInfo { get; set; }
        
        // Receipt Info (n?u ?ã nh?n)
        public ReceiptInfoDto? ReceiptInfo { get; set; }
        
        // Issues (n?u có v?n ??)
        public List<PartOrderIssueDto>? Issues { get; set; }
        
        // Discrepancy Resolution (n?u có sai l?ch)
        public DiscrepancyResolutionDto? DiscrepancyResolution { get; set; }
    }

    public class PartOrderItemDetailDto
    {
        public Guid OrderItemId { get; set; }
        public string Model { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public string? Remarks { get; set; }
        public int ScStock { get; set; }
        public int OemStock { get; set; }
    }

    public class ShipmentInfoDto
    {
        public DateTime ShippedAt { get; set; }
        public int TotalItemsShipped { get; set; }
        public List<ShipmentItemDto> Items { get; set; } = new List<ShipmentItemDto>();
    }

    public class ShipmentItemDto
    {
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string Status { get; set; } // Confirmed, Pending
    }

    public class ReceiptInfoDto
    {
        public DateTime? ReceivedAt { get; set; }
        public int TotalItemsReceived { get; set; }
        public int GoodItems { get; set; }
        public int DamagedItems { get; set; }
        public List<ReceiptItemDto> Items { get; set; } = new List<ReceiptItemDto>();
    }

    public class ReceiptItemDto
    {
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string Status { get; set; } // Received, Damaged
        public string? Note { get; set; }
        public string? ImageUrl { get; set; }
    }
}
