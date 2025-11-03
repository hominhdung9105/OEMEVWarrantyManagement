using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IPartOrderRepository
    {
        Task<PartOrder> CreateAsync(PartOrder Request);
        Task<IEnumerable<PartOrder>> GetAll();
        Task<PartOrder> GetPartOrderByIdAsync(Guid id);
        Task<PartOrder> UpdateAsync(PartOrder Request);
        Task<PartOrder> GetPendingPartOrderByOrgIdAsync(Guid orgId);
        Task<(IEnumerable<PartOrder> Data, int TotalRecords)> GetPagedPartOrderByOrdIdAsync(int pageNumber, int pageSize, Guid orgId);
    }
}
