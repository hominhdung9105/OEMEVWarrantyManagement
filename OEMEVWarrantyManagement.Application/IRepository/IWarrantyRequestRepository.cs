using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IWarrantyRequestRepository
    {
        Task<WarrantyRequest> CreateAsync(WarrantyRequest request);
        Task<IEnumerable<WarrantyRequest>> GetAllAsync();
        Task<WarrantyRequest> GetByIdAsync(Guid id);
        Task<WarrantyRequest> UpdateAsync(WarrantyRequest Request);
        Task<WarrantyRequest> DeleteAsync(Guid id);
    }
}
