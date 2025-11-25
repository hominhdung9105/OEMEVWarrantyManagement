using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IPartOrderIssueRepository
    {
        Task<PartOrderIssue> CreateAsync(PartOrderIssue issue);
        Task<IEnumerable<PartOrderIssue>> GetByOrderIdAsync(Guid orderId);
        Task<PartOrderIssue?> GetLatestByOrderIdAsync(Guid orderId);
    }

    public interface IPartOrderDiscrepancyResolutionRepository
    {
        Task<PartOrderDiscrepancyResolution> CreateAsync(PartOrderDiscrepancyResolution resolution);
        Task<PartOrderDiscrepancyResolution?> GetByOrderIdAsync(Guid orderId);
        Task<PartOrderDiscrepancyResolution> UpdateAsync(PartOrderDiscrepancyResolution resolution);
        Task<IEnumerable<PartOrderDiscrepancyResolution>> GetPendingResolutionsAsync();
        Task<PartOrderDiscrepancyDetail> CreateDetailAsync(PartOrderDiscrepancyDetail detail);
        Task<IEnumerable<PartOrderDiscrepancyDetail>> GetDetailsByResolutionIdAsync(Guid resolutionId);
    }
}
