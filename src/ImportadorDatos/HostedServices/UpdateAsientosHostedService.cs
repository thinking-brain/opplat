using System;
using System.Threading;
using System.Threading.Tasks;
using ImportadorDatos.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ImportadorDatos.HostedServices
{
    public class UpdateAsientosHostedService : BackgroundService
    {
        //ImportarVersat _importador;
        public IServiceProvider Services { get; }
        ILogger<UpdateAsientosHostedService> _logger;

        public UpdateAsientosHostedService(IServiceProvider service, ILogger<UpdateAsientosHostedService> logger)
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

                    scopedProcessingService.ImportarAsientos();
                }
                await Task.Delay(300000, stoppingToken);
            }
        }
    }
}