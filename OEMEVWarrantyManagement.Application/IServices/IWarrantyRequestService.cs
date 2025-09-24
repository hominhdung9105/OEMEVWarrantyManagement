using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWarrantyRequestService
    {
        Task<WarrantyRequestDto> CreateAsync(WarrantyRequestDto request);
        Task<IEnumerable<WarrantyRequestDto>> GetAllAsync();
        Task<WarrantyRequestDto> GetByIdAsync(Guid id);
        Task<WarrantyRequestDto> UpdateAsync(WarrantyRequestDto Request);
        Task<bool> DeleteAsync(Guid id);
    }
}
