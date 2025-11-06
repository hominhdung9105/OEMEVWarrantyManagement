using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using System.Collections.Generic;
using System;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IPartOrderService
    {
        Task<PartOrderDto> GetByIdAsync(Guid id);
        Task<PartOrderDto> UpdateStatusAsync(Guid id, PartOrderStatus status);
        Task<PagedResult<ResponsePartOrderDto>> GetPagedPartOrderForEvmStaffAsync(PaginationRequest request, string? search = null);
        Task<PagedResult<ResponsePartOrderForScStaffDto>> GetPagedPartOrderForScStaffAsync(PaginationRequest request);
        Task<bool> UpdateExpectedDateAsync(Guid id, UpdateExpectedDateDto dto);
        // Count orders by status with optional org scope
        Task<int> CountByStatusAsync(PartOrderStatus status, Guid? orgId = null);
        // Convenience: count pending across all orgs
        Task<int> CountPendingAsync();
        // New: top requested parts within month or year
        Task<IEnumerable<PartRequestedTopDto>> GetTopRequestedPartsAsync(int? month, int? year, int take = 5);
    }
}
