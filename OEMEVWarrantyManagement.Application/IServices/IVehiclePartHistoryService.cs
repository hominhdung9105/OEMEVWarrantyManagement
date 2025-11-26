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
        // Updated: accept unified search (vin|model|orgId) and condition & status filters
        Task<PagedResult<ResponseVehiclePartHistoryDto>> GetPagedAsync(PaginationRequest request, string? search = null, string? condition = null, string? status = null);
    }
}