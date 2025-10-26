using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IVehicleService
    {
        Task<PagedResult<ResponseVehicleDto>> GetPagedAsync(PaginationRequest request);
    }
}
