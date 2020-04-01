using System;
using System.Threading;
using System.Threading.Tasks;
using kube_consul_registrator.Configurations;
using Microsoft.Extensions.Logging;

namespace kube_consul_registrator.Services 
{
    public class ConsulRegisterCronJob : CronJobService 
    {
        private readonly ILogger<ConsulRegisterCronJob> _logger;

        public ConsulRegisterCronJob (IScheduleConfig<ConsulRegisterCronJob> config, ILogger<ConsulRegisterCronJob> logger) 
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Consul register cronjob started");
            return base.StartAsync(cancellationToken);
        }

        public override Task RunWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Consul register cronjob is working");

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consul register job is stopping");
            return base.StopAsync(cancellationToken);
        }


    }
}