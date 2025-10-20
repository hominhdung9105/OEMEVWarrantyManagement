using OEMEVWarrantyManagement.Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IVehicleWarrantyPolicyService
    {
        Task<IEnumerable<VehicleWarrantyPolicyDto>> GetAllByVinAsync(string vin);
    }
}
