using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using kube_consul_registrator.Configurations;
using kube_consul_registrator.Models;
using kube_consul_registrator.Repositories;
using Microsoft.Extensions.Logging;

namespace kube_consul_registrator.Services 
{
    public class ConsulRegisterCronJob : CronJobService 
    {
        private readonly IKubernetesRepository _kubeRepo;
        private readonly ILogger<ConsulRegisterCronJob> _logger;

        public ConsulRegisterCronJob (IScheduleConfig<ConsulRegisterCronJob> config, IKubernetesRepository kubeRepo, ILogger<ConsulRegisterCronJob> logger) 
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _kubeRepo = kubeRepo;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Consul register cronjob started");
            return base.StartAsync(cancellationToken);
        }

        public override async Task RunWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Gethering pod information from kubernetes");
            
            var pods = await _kubeRepo.GetPods("qe-tools");

            
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consul register job is stopping");
            return base.StopAsync(cancellationToken);
        }
    }
}