using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IVehicleRepository
    {
        Task<Vehicle> GetVehicleByVinAsync(string Vin);
        Task<(IEnumerable<Vehicle> Data, int TotalRecords)> GetPagedVehicleAsync(int pageNumber, int pageSize, string? search);
        Task<List<Vehicle>> GetVehiclesByVinsAsync(List<string> vins);
        Task<IEnumerable<Vehicle>> GetAllAsync();
    }
}
