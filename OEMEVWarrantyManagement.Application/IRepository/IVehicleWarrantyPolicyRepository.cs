using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IVehicleWarrantyPolicyRepository
    {
        Task<IEnumerable<VehicleWarrantyPolicy>> GetAllVehicleWarrantyPolicyByVinAsync(string vin);
        Task<VehicleWarrantyPolicy> GetByIdAsync(Guid vehicleWarrantyId);
    }
}
