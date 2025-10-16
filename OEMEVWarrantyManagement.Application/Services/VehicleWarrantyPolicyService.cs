using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class VehicleWarrantyPolicyService : IVehicleWarrantyPolicyService
    {
        public Task<VehicleWarrantyPolicyDto> GetAllByVinAsync(string vin)
        {
            throw new NotImplementedException();
        }
    }
}
