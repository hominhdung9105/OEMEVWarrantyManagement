using OEMEVWarrantyManagement.Application.IServices;

namespace OEMEVWarrantyManagement.API.BackgroundServices
{
    /// <summary>
    /// Background service that runs daily to automatically close expired campaigns
    /// Runs at 8:00 AM UTC every day
    /// </summary>
    public class CampaignAutoCloseBackgroundService : BackgroundService
    {
        private readonly ILogger<CampaignAutoCloseBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Check every 24 hours
        private readonly TimeSpan _dailyRunTime = new TimeSpan(0, 0, 0); // Run at 8:00 AM UTC

        public CampaignAutoCloseBackgroundService(
            ILogger<CampaignAutoCloseBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Campaign Auto-Close Background Service is starting.");

            // Wait until the first scheduled run time
            await WaitUntilNextRunTime(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Campaign Auto-Close Background Service is running at {Time}", DateTime.UtcNow);

                    // Create a scope to resolve scoped services
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var campaignService = scope.ServiceProvider.GetRequiredService<ICampaignService>();
                        
                        await campaignService.AutoCloseExpiredCampaignsAsync();
                    }

                    _logger.LogInformation("Campaign Auto-Close Background Service completed successfully at {Time}", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while executing Campaign Auto-Close Background Service at {Time}", DateTime.UtcNow);
                }

                // Wait until the next scheduled run time (24 hours from now)
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Campaign Auto-Close Background Service is stopping.");
        }

        /// <summary>
        /// Calculate delay until next run time (8:00 AM UTC)
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
            
            _logger.LogInformation("Campaign Auto-Close Background Service will run in {Delay} at {NextRunTime}", 
                delay, nextRunTime);

            if (delay.TotalMilliseconds > 0)
            {
                await Task.Delay(delay, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Campaign Auto-Close Background Service is stopping gracefully.");
            await base.StopAsync(cancellationToken);
        }
    }
}
