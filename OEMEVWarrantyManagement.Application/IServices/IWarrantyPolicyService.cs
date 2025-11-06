using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IWarrantyPolicyService
    {
        Task<IEnumerable<WarrantyPolicyDto>> GetAllAsync();
        Task<PagedResult<WarrantyPolicyDto>> GetAllAsync(PaginationRequest request);
    }
}
