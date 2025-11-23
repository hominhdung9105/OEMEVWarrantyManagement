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
        // Updated: add condition & status filters
        Task<PagedResult<VehiclePartHistoryDto>> GetPagedAsync(PaginationRequest request, string? vin = null, string? model = null, string? condition = null, string? status = null);
    }
}