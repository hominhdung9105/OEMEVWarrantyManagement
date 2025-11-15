using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OEMEVWarrantyManagement.Application.IServices;

namespace OEMEVWarrantyManagement.Infrastructure.BackgroundServices
{
    public class CampaignReminderBackgroundService : BackgroundService
    {
        private readonly ILogger<CampaignReminderBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Run once per day

        public CampaignReminderBackgroundService(
            ILogger<CampaignReminderBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Campaign Reminder Background Service is starting");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Campaign Reminder Background Service is running at: {time}", DateTimeOffset.Now);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var notificationService = scope.ServiceProvider.GetRequiredService<ICampaignNotificationService>();
                        await notificationService.SendReminderEmailsAsync();
                    }

                    _logger.LogInformation("Campaign Reminder Background Service completed at: {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in Campaign Reminder Background Service");
                }

                // Wait for the next interval
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Campaign Reminder Background Service is stopping");
        }
    }
}
