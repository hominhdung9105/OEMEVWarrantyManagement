using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;

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
        Task<int> CountByStatusAsync(PartOrderStatus status, Guid? orgId = null);
        Task<IEnumerable<(string Model, int Quantity)>> GetTopRequestedPartsAsync(DateTime fromDate, DateTime toDate, int take);
    }
}
