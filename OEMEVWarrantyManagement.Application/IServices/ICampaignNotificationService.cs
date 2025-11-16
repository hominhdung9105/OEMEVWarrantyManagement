using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface ICampaignNotificationService
    {
        Task ProcessCampaignNotificationsAsync(Guid campaignId);
        Task SendReminderEmailsAsync();
        Task MarkVehicleAsCompletedAsync(Guid campaignId, string vin);
        Task<List<CampaignVehicleStatusDto>> GetCampaignVehicleStatusesAsync(Guid campaignId);
    }
}
