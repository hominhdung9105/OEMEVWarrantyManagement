using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface ICampaignNotificationRepository
    {
        Task<CampaignNotification?> GetByCampaignAndVinAsync(Guid campaignId, string vin);
        Task<List<CampaignNotification>> GetByCampaignIdAsync(Guid campaignId);
        Task<CampaignNotification> CreateAsync(CampaignNotification notification);
        Task<CampaignNotification> UpdateAsync(CampaignNotification notification);
        Task<List<CampaignNotification>> GetPendingNotificationsAsync(int daysInterval);
        Task<List<CampaignNotification>> CreateRangeAsync(List<CampaignNotification> notifications);
    }
}
