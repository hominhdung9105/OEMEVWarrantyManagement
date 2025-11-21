using Microsoft.Extensions.Logging;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class CampaignNotificationService : ICampaignNotificationService
    {
        private readonly ICampaignNotificationRepository _notificationRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IVehiclePartRepository _vehiclePartRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ICampaignVehicleRepository _campaignVehicleRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<CampaignNotificationService> _logger;

        public CampaignNotificationService(
            ICampaignNotificationRepository notificationRepository,
            ICampaignRepository campaignRepository,
            IVehicleRepository vehicleRepository,
            IVehiclePartRepository vehiclePartRepository,
            ICustomerRepository customerRepository,
            IAppointmentRepository appointmentRepository,
            ICampaignVehicleRepository campaignVehicleRepository,
            IEmailService emailService,
            ILogger<CampaignNotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _campaignRepository = campaignRepository;
            _vehicleRepository = vehicleRepository;
            _vehiclePartRepository = vehiclePartRepository;
            _customerRepository = customerRepository;
            _appointmentRepository = appointmentRepository;
            _campaignVehicleRepository = campaignVehicleRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ProcessCampaignNotificationsAsync(Guid campaignId)
        {
            try
            {
                var campaign = await _campaignRepository.GetByIdAsync(campaignId);
                if (campaign == null || string.IsNullOrWhiteSpace(campaign.PartModel))
                {
                    _logger.LogWarning("Campaign {CampaignId} not found or has no part model", campaignId);
                    return;
                }

                var vehicles = await _vehicleRepository.GetAllAsync();
                var affectedVehicles = new List<string>();

                foreach (var vehicle in vehicles)
                {
                    if (await _vehiclePartRepository.ExistsByVinAndModelAsync(vehicle.Vin, campaign.PartModel))
                    {
                        affectedVehicles.Add(vehicle.Vin);
                    }
                }

                if (!affectedVehicles.Any())
                {
                    _logger.LogInformation("No affected vehicles found for campaign {CampaignId}", campaignId);
                    return;
                }

                // Create notification records for tracking
                var notifications = new List<CampaignNotification>();
                var now = DateTime.UtcNow;

                foreach (var vin in affectedVehicles)
                {
                    // Check if notification record already exists
                    var existing = await _notificationRepository.GetByCampaignAndVinAsync(campaignId, vin);
                    if (existing == null)
                    {
                        notifications.Add(new CampaignNotification
                        {
                            CampaignNotificationId = Guid.NewGuid(),
                            CampaignId = campaignId,
                            Vin = vin,
                            EmailSentCount = 0,
                            IsCompleted = false,
                            CreatedAt = now
                        });
                    }
                }

                if (notifications.Any())
                {
                    await _notificationRepository.CreateRangeAsync(notifications);
                    _logger.LogInformation("Created {Count} notification records for campaign {CampaignId}", 
                        notifications.Count, campaignId);
                }

                // Send initial emails asynchronously (fire-and-forget)
                //_ = Task.Run(async () => await SendInitialEmailsAsync(campaignId, campaign));
                await SendInitialEmailsAsync(campaignId, campaign);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing campaign notifications for campaign {CampaignId}", campaignId);
            }
        }

        private async Task SendInitialEmailsAsync(Guid campaignId, Campaign campaign)
        {
            try
            {
                var notifications = await _notificationRepository.GetByCampaignIdAsync(campaignId);
                var now = DateTime.UtcNow;

                foreach (var notification in notifications.Where(n => n.EmailSentCount == 0))
                {
                    try
                    {
                        var vehicle = await _vehicleRepository.GetVehicleByVinAsync(notification.Vin);
                        if (vehicle == null) continue;

                        var customer = await _customerRepository.GetCustomerByIdAsync(vehicle.CustomerId);
                        if (customer == null || string.IsNullOrWhiteSpace(customer.Email)) continue;

                        await _emailService.SendCampaignPartIssueEmailAsync(
                            to: customer.Email,
                            customerName: customer.Name,
                            vin: vehicle.Vin,
                            partModel: campaign.PartModel!,
                            campaignTitle: campaign.Title,
                            note: campaign.Description,
                            bookingUrl: null
                        );

                        // Update notification record
                        notification.EmailSentCount = 1;
                        notification.FirstEmailSentAt = now;
                        notification.LastEmailSentAt = now;
                        notification.UpdatedAt = now;
                        await _notificationRepository.UpdateAsync(notification);

                        _logger.LogInformation("Sent initial email for VIN {Vin} in campaign {CampaignId}", 
                            notification.Vin, campaignId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send email for VIN {Vin} in campaign {CampaignId}", 
                            notification.Vin, campaignId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending initial emails for campaign {CampaignId}", campaignId);
            }
        }

        public async Task SendReminderEmailsAsync()
        {
            try
            {
                const int reminderIntervalDays = 7;
                var pendingNotifications = await _notificationRepository.GetPendingNotificationsAsync(reminderIntervalDays);

                _logger.LogInformation("Found {Count} pending notifications to process", pendingNotifications.Count);

                foreach (var notification in pendingNotifications)
                {
                    try
                    {
                        // Check IsCompleted flag first (fastest check)
                        if (notification.IsCompleted)
                        {
                            _logger.LogInformation("Skipping reminder for VIN {Vin} - already completed", notification.Vin);
                            continue;
                        }

                        // Check if vehicle should receive reminder
                        var shouldSend = await ShouldSendReminderAsync(notification);
                        if (!shouldSend)
                        {
                            _logger.LogInformation("Skipping reminder for VIN {Vin} - vehicle in campaign or has scheduled appointment after email time", notification.Vin);
                            continue;
                        }

                        var vehicle = notification.Vehicle;
                        var customer = vehicle?.Customer;
                        var campaign = notification.Campaign;

                        if (customer == null || string.IsNullOrWhiteSpace(customer.Email) || campaign == null)
                        {
                            _logger.LogWarning("Missing customer or campaign data for notification {Id}", 
                                notification.CampaignNotificationId);
                            continue;
                        }

                        await _emailService.SendCampaignPartIssueEmailAsync(
                            to: customer.Email,
                            customerName: customer.Name,
                            vin: vehicle.Vin,
                            partModel: campaign.PartModel!,
                            campaignTitle: campaign.Title,
                            note: campaign.Description,
                            bookingUrl: null
                        );

                        // Update notification record
                        var now = DateTime.UtcNow;
                        notification.EmailSentCount += 1;
                        notification.LastEmailSentAt = now;
                        notification.UpdatedAt = now;
                        if (notification.FirstEmailSentAt == null)
                        {
                            notification.FirstEmailSentAt = now;
                        }
                        await _notificationRepository.UpdateAsync(notification);

                        _logger.LogInformation("Sent reminder #{Count} for VIN {Vin} in campaign {CampaignId}", 
                            notification.EmailSentCount, notification.Vin, notification.CampaignId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send reminder for notification {Id}", 
                            notification.CampaignNotificationId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendReminderEmailsAsync");
            }
        }

        private async Task<bool> ShouldSendReminderAsync(CampaignNotification notification)
        {
            try
            {
                // STEP 1: Check if vehicle already exists in CampaignVehicle
                var existsInCampaignVehicle = await _campaignVehicleRepository.ExistsByCampaignAndVinAsync(
                    notification.CampaignId, 
                    notification.Vin);

                if (existsInCampaignVehicle)
                {
                    // Vehicle is already in campaign processing -> Don't send reminder
                    return false;
                }

                // STEP 2: Check appointment status
                var appointments = await _appointmentRepository.GetAppointmentsByVinAsync(notification.Vin);
                
                if (!appointments.Any())
                {
                    // No appointments -> Send reminder
                    return true;
                }

                // Get latest Campaign type appointment
                var latestCampaignAppointment = appointments
                    .Where(a => a.AppointmentType == "Campaign")
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefault();

                if (latestCampaignAppointment == null)
                {
                    // No Campaign appointments -> Send reminder
                    return true;
                }

                var status = latestCampaignAppointment.Status;
                var scheduledStatus = AppointmentStatus.Scheduled.GetAppointmentStatus();

                // If appointment is Scheduled AND appointment date is after current date -> Skip
                if (status == scheduledStatus)
                {
                    var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
                    if (latestCampaignAppointment.AppointmentDate > currentDate)
                    {
                        // Appointment is scheduled for future date -> Skip (customer has responded)
                        return false;
                    }
                }

                // All other cases -> Send reminder
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if should send reminder for notification {Id}", 
                    notification.CampaignNotificationId);
                return false;
            }
        }

        public async Task MarkVehicleAsCompletedAsync(Guid campaignId, string vin)
        {
            try
            {
                var notification = await _notificationRepository.GetByCampaignAndVinAsync(campaignId, vin);
                if (notification != null && !notification.IsCompleted)
                {
                    notification.IsCompleted = true;
                    notification.UpdatedAt = DateTime.UtcNow;
                    await _notificationRepository.UpdateAsync(notification);
                    _logger.LogInformation("Marked notification as completed for VIN {Vin} in campaign {CampaignId}", 
                        vin, campaignId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking vehicle as completed for VIN {Vin} in campaign {CampaignId}", 
                    vin, campaignId);
            }
        }

        public async Task<List<CampaignVehicleStatusDto>> GetCampaignVehicleStatusesAsync(Guid campaignId)
        {
            try
            {
                var campaign = await _campaignRepository.GetByIdAsync(campaignId);
                if (campaign == null)
                {
                    _logger.LogWarning("Campaign {CampaignId} not found", campaignId);
                    return new List<CampaignVehicleStatusDto>();
                }

                // Get all notifications for this campaign
                var notifications = await _notificationRepository.GetByCampaignIdAsync(campaignId);
                var result = new List<CampaignVehicleStatusDto>();

                foreach (var notification in notifications)
                {
                    var vehicle = await _vehicleRepository.GetVehicleByVinAsync(notification.Vin);
                    if (vehicle == null) continue;

                    var customer = await _customerRepository.GetCustomerByIdAsync(vehicle.CustomerId);
                    
                    var status = new CampaignVehicleStatusDto
                    {
                        Vin = notification.Vin,
                        Model = vehicle.Model,
                        Year = vehicle.Year,
                        CustomerName = customer?.Name ?? "N/A",
                        CustomerEmail = customer?.Email ?? "N/A",
                        CustomerPhone = customer?.Phone ?? "N/A",
                        EmailSentCount = notification.EmailSentCount,
                        OverallStatus = "NoResponse" // Default
                    };

                    // Check CampaignVehicle status
                    var campaignVehicles = await _campaignVehicleRepository.GetByCampaignAndVinsAsync(
                        campaignId, 
                        new[] { notification.Vin });
                    
                    var campaignVehicle = campaignVehicles.FirstOrDefault();
                    if (campaignVehicle != null)
                    {
                        status.OverallStatus = campaignVehicle.Status == "Done" ? "Completed" : "InProgress";
                    }
                    else
                    {
                        // Check Campaign Appointment if not in campaign vehicle
                        var appointments = await _appointmentRepository.GetAppointmentsByVinAsync(notification.Vin);
                        var hasCampaignAppointment = appointments
                            .Any(a => a.AppointmentType == "Campaign");

                        if (hasCampaignAppointment)
                        {
                            status.OverallStatus = "HasAppointment";
                        }
                    }

                    result.Add(status);
                }

                return result.OrderByDescending(s => s.EmailSentCount)
                             .ThenBy(s => s.OverallStatus)
                             .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaign vehicle statuses for campaign {CampaignId}", campaignId);
                return new List<CampaignVehicleStatusDto>();
            }
        }
    }
}
