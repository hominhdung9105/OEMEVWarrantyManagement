using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IVehiclePartHistoryService
    {
        Task<IEnumerable<VehiclePartHistoryDto>> GetHistoryByVinAsync(string vin);
        Task<IEnumerable<VehiclePartHistoryDto>> GetHistoryByVinAndModelAsync(string vin, string model);
        Task<VehicleWithHistoryDto> GetVehicleWithHistoryAsync(string vin, string? model = null); // new method
    }
}