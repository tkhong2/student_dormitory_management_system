using BillingMaintenanceService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace BillingMaintenanceService.Application.Services
{
    public class MonthlyInvoiceBatchService : BackgroundService
    {
        private readonly ILogger<MonthlyInvoiceBatchService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private int _lastGeneratedMonth = 0;
        private int _lastGeneratedYear = 0;

        public MonthlyInvoiceBatchService(ILogger<MonthlyInvoiceBatchService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Monthly invoice batch service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var utcNow = DateTime.UtcNow;
                    var currentMonth = utcNow.Month;
                    var currentYear = utcNow.Year;

                    if ((_lastGeneratedYear != currentYear || _lastGeneratedMonth != currentMonth) && utcNow.Day == 1)
                    {
                        _logger.LogInformation("Running monthly invoice generation for {Month}/{Year}.", currentMonth, currentYear);
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
                            var invoices = await invoiceService.GenerateMonthlyInvoicesForMonthAsync(currentMonth, currentYear, createdByUserId: 0);
                            _logger.LogInformation("Monthly invoice generation completed. Created {Count} invoices.", invoices.Count());
                        }
                        _lastGeneratedMonth = currentMonth;
                        _lastGeneratedYear = currentYear;
                    }

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
                        var overdueCount = await invoiceService.ProcessOverdueInvoicesAsync(7);
                        if (overdueCount > 0)
                        {
                            _logger.LogInformation("Processed {Count} overdue invoices.", overdueCount);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while running monthly invoice batch.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
