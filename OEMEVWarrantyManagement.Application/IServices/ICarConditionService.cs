using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface ICarConditionService
    {
        Task<IEnumerable<CarConditionCurrentDto>> GetAllAsync();
        Task<IEnumerable<CarConditionCurrentDto>> GetAllByStaffAsync(string staffId);
        Task<CarConditionCurrentDto?> GetAsync(string warrantyRequestId);
        //Task<CarConditionCurrentDto?> CreateAsync(string warrantyRequestId);
        Task<CarConditionCurrentDto?> UpdateAsync(string staffId, string warrantyRequestId, CarConditionCurrentDto request);

    }
}
