using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace RentaAPIs.Services
{
    public class RentMonitoringBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RentMonitoringBackgroundService>
            _logger;

        public RentMonitoringBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<RentMonitoringBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            // Run at startup and then once every 24 hours.
            // A fixed interval does not guarantee a specific
            // South African clock time.
            using var timer = new PeriodicTimer(
                TimeSpan.FromHours(24));

            do
            {
                try
                {
                    using var scope =
                        _scopeFactory.CreateScope();

                    var service = scope.ServiceProvider
                        .GetRequiredService<RentMonitoringService>();

                    var changes =
                        await service.MonitorRentPayments();

                    _logger.LogInformation(
                        "Rent monitoring completed. " +
                        "{Changes} records changed.",
                        changes);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Rent monitoring failed.");
                }
            }
            while (await timer.WaitForNextTickAsync(
                stoppingToken));
        }
    }
}

