using System;
using System.Threading;
using System.Threading.Tasks;
using ImportadorDatos.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ImportadorDatos.HostedServices
{
    public class UpdateCuentasHostedService : BackgroundService
    {
        //ImportarVersat _importador;
        public IServiceProvider Services { get; }
        ILogger<UpdateCuentasHostedService> _logger;

        public UpdateCuentasHostedService(IServiceProvider service, ILogger<UpdateCuentasHostedService> logger)
        {
            _logger = logger;
            Services = service;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Tarea ejecutada: {DateTime.Now}");
                using (var scope = Services.CreateScope())
                {
                    var scopedProcessingService =
                        scope.ServiceProvider
                            .GetRequiredService<ImportarVersat>();

                    scopedProcessingService.ImportarCuentasAsync();
                }
                await Task.Delay(86400000, stoppingToken);
            }
        }
    }
}