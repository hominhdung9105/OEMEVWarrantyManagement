using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface ICampaignRepository
    {
        Task<int> CountByStatusAsync(string status);
        Task<int> CountCampaignVehiclesByStatusAsync(string status);
        Task<int> CountCampaignVehiclesNotInStatusAsync(string status);
    }
}
