using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OEMEVWarrantyManagement.Application.IServices;

namespace OEMEVWarrantyManagement.Application.BackgroundServices
{
    /// <summary>
    /// Background service that runs daily to check and send campaign email reminders
    /// Checks every day at a specific time (default: 9:00 AM UTC)
    /// </summary>
    public class CampaignReminderBackgroundService : BackgroundService
    {
        private readonly ILogger<CampaignReminderBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Check every 24 hours
        private readonly TimeSpan _dailyRunTime = new TimeSpan(9, 0, 0); // Run at 9:00 AM UTC

        public CampaignReminderBackgroundService(
            ILogger<CampaignReminderBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Campaign Reminder Background Service is starting.");

            // Wait until the first scheduled run time
            await WaitUntilNextRunTime(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Campaign Reminder Background Service is running at {Time}", DateTime.UtcNow);

                    // Create a scope to resolve scoped services
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var notificationService = scope.ServiceProvider.GetRequiredService<ICampaignNotificationService>();
                        
                        await notificationService.SendReminderEmailsAsync();
                    }

                    _logger.LogInformation("Campaign Reminder Background Service completed successfully at {Time}", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while executing Campaign Reminder Background Service at {Time}", DateTime.UtcNow);
                }

                // Wait until the next scheduled run time (24 hours from now)
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Campaign Reminder Background Service is stopping.");
        }

        /// <summary>
        /// Calculate delay until next run time (9:00 AM UTC)
        /// </summary>
        private async Task WaitUntilNextRunTime(CancellationToken stoppingToken)
        {
            var now = DateTime.UtcNow;
            var nextRunTime = now.Date.Add(_dailyRunTime);

            // If the time has already passed today, schedule for tomorrow
            if (now > nextRunTime)
            {
                nextRunTime = nextRunTime.AddDays(1);
            }

            var delay = nextRunTime - now;
            
            _logger.LogInformation("Campaign Reminder Background Service will run in {Delay} at {NextRunTime}", 
                delay, nextRunTime);

            if (delay.TotalMilliseconds > 0)
            {
                await Task.Delay(delay, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Campaign Reminder Background Service is stopping gracefully.");
            await base.StopAsync(cancellationToken);
        }
    }
}
