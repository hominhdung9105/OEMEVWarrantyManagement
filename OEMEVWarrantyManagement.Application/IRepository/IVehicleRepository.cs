using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IVehicleRepository
    {
        Task<Vehicle> GetVehicleByVinAsync(string Vin);
        Task<IEnumerable <Vehicle>> GetAllVehicleAsync();
        Task<List<Vehicle>> GetVehiclesByVinsAsync(List<string> vins);
    }
}
