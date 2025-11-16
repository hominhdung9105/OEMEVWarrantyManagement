using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class CampaignNotificationRepository : ICampaignNotificationRepository
    {
        private readonly AppDbContext _context;

        public CampaignNotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CampaignNotification?> GetByCampaignAndVinAsync(Guid campaignId, string vin)
        {
            return await _context.CampaignNotifications
                .FirstOrDefaultAsync(cn => cn.CampaignId == campaignId && cn.Vin == vin);
        }

        public async Task<List<CampaignNotification>> GetByCampaignIdAsync(Guid campaignId)
        {
            return await _context.CampaignNotifications
                .Where(cn => cn.CampaignId == campaignId)
                .ToListAsync();
        }

        public async Task<CampaignNotification> CreateAsync(CampaignNotification notification)
        {
            await _context.CampaignNotifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<List<CampaignNotification>> CreateRangeAsync(List<CampaignNotification> notifications)
        {
            await _context.CampaignNotifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
            return notifications;
        }

        public async Task<CampaignNotification> UpdateAsync(CampaignNotification notification)
        {
            _context.CampaignNotifications.Update(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<List<CampaignNotification>> GetPendingNotificationsAsync(int daysInterval)
        {
            var now = DateTime.UtcNow;
            var activeCampaignStatus = CampaignStatus.Active.GetCampaignStatus();

            // Get notifications that need to be sent:
            // 1. Not completed (IsCompleted = false)
            // 2. Campaign is Active
            // 3. Either never sent email (FirstEmailSentAt is null) OR 
            //    last email was sent >= daysInterval days ago (calculated per vehicle)
            var pendingNotifications = await _context.CampaignNotifications
                .Include(cn => cn.Campaign)
                .Include(cn => cn.Vehicle)
                    .ThenInclude(v => v.Customer)
                .Where(cn => 
                    !cn.IsCompleted && 
                    cn.Campaign.Status == activeCampaignStatus &&
                    cn.EmailSentCount < 3 && // Limit to max 3 emails
                    (cn.LastEmailSentAt == null || 
                     EF.Functions.DateDiffDay(cn.LastEmailSentAt.Value, now) >= daysInterval))
                .ToListAsync();

            return pendingNotifications;
        }
    }
}
