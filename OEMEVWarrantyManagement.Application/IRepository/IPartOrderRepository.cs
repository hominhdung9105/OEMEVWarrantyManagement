using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;
using System.Collections.Generic;
using System;

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
        // Count number of orders by status with optional org filter
        Task<int> CountByStatusAsync(PartOrderStatus status, Guid? orgId = null);
        // New: top requested parts within date range
        Task<IEnumerable<(string Model, int Quantity)>> GetTopRequestedPartsAsync(DateTime fromDate, DateTime toDate, int take);
    }
}
