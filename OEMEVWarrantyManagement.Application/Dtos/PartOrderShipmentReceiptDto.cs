namespace OEMEVWarrantyManagement.Application.Dtos
{
    /// <summary>
    /// DTO ??i di?n cho m?t dòng trong file Excel g?i hàng (EVM)
    /// Format: Model | SerialNumber
    /// </summary>
    public class ShipmentExcelRowDto
    {
        public string Model { get; set; }
        public string SerialNumber { get; set; }
    }

    /// <summary>
    /// DTO ??i di?n cho m?t dòng trong file Excel nh?n hàng (SC)
    /// Format: Model | SerialNumber
    /// </summary>
    public class ReceiptExcelRowDto
    {
        public string Model { get; set; }
        public string SerialNumber { get; set; }
    }

    /// <summary>
    /// DTO cho ph?n h?i k?t qu? validate file g?i hàng
    /// </summary>
    public class ShipmentValidationResultDto
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public Dictionary<string, QuantityDiscrepancyDto> QuantityDiscrepancies { get; set; } = new Dictionary<string, QuantityDiscrepancyDto>();
        public List<SerialErrorDto> SerialErrors { get; set; } = new List<SerialErrorDto>();
    }

    /// <summary>
    /// DTO cho s? chênh l?ch s? l??ng
    /// </summary>
    public class QuantityDiscrepancyDto
    {
        public string Model { get; set; }
        public int Requested { get; set; }
        public int Provided { get; set; }
        public int Difference { get; set; } // Positive = th?a, Negative = thi?u
        public string Status { get; set; } // "Match", "Excess", "Shortage"
    }

    /// <summary>
    /// DTO cho l?i serial
    /// </summary>
    public class SerialErrorDto
    {
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string ErrorType { get; set; } // "NotInStock", "WrongModel", "Duplicate", "AlreadyShipped"
        public string Message { get; set; }
    }

    /// <summary>
    /// DTO cho ph?n h?i k?t qu? validate file nh?n hàng
    /// </summary>
    public class ReceiptValidationResultDto
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public Dictionary<string, QuantityDiscrepancyDto> QuantityDiscrepancies { get; set; } = new Dictionary<string, QuantityDiscrepancyDto>();
        public List<SerialMismatchDto> SerialMismatches { get; set; } = new List<SerialMismatchDto>();
    }

    /// <summary>
    /// DTO cho serial không kh?p khi nh?n hàng
    /// </summary>
    public class SerialMismatchDto
    {
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string ErrorType { get; set; } // "NotShipped", "Duplicate", "ExtraSerial"
        public string Message { get; set; }
    }

    /// <summary>
    /// DTO cho báo cáo ph? tùng h? h?ng
    /// </summary>
    public class DamagedPartReportDto
    {
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string Note { get; set; }
        public string? ImageUrl { get; set; }
    }

    /// <summary>
    /// Request DTO ?? xác nh?n nh?n hàng hoàn t?t
    /// </summary>
    public class ConfirmReceiptRequestDto
    {
        public Guid OrderId { get; set; }
        public List<DamagedPartReportDto>? DamagedParts { get; set; }
    }
}
