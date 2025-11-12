using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IVehicleWarrantyPolicyRepository
    {
        Task<IEnumerable<VehicleWarrantyPolicy>> GetAllVehicleWarrantyPolicyByVinAsync(string vin);
        Task<VehicleWarrantyPolicy> GetByIdAsync(Guid vehicleWarrantyId);
    }
}
