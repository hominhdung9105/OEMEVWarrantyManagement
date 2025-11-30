using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IVehicleWarrantyPolicyService
    {
        Task<IEnumerable<VehicleWarrantyPolicyDto>> GetAllByVinAsync(string vin);
    }
}
