using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IVehiclePartHistoryService
    {
        Task<IEnumerable<VehiclePartHistoryDto>> GetHistoryByVinAsync(string vin);
        Task<IEnumerable<VehiclePartHistoryDto>> GetHistoryByVinAndModelAsync(string vin, string model);
        Task<VehicleWithHistoryDto> GetVehicleWithHistoryAsync(string vin, string? model = null); // new method
        Task<IEnumerable<string>> GetSerialsByVinAndPartModelAsync(string vin, string partModel);
        // Updated: use 'search' parameter to filter across both VIN and model fields, plus condition, status & serviceCenterId filters
        Task<PagedResult<ResponseVehiclePartHistoryDto>> GetPagedAsync(PaginationRequest request, string? search = null, string? condition = null, string? status = null, Guid? serviceCenterId = null);
        
        // New: Get available serials in stock for current user's org by model
        Task<IEnumerable<string>> GetAvailableSerialsByModelAsync(string model);
        
        // New: Validate serial in stock for repair
        Task ValidateSerialForRepairAsync(string model, string serialNumber);
        
        // New: Get parts in transit to an organization (for tracking incoming shipments)
        Task<IEnumerable<VehiclePartHistoryDto>> GetInTransitPartsAsync();
        
        // New: Get parts in transit for a specific order
        Task<IEnumerable<VehiclePartHistoryDto>> GetInTransitPartsByOrderAsync(Guid orderId);
    }
}