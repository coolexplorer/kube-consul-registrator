using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Consul;
using kube_consul_registrator.Configurations;
using kube_consul_registrator.Dtos;
using kube_consul_registrator.Helpers;
using kube_consul_registrator.Models;
using kube_consul_registrator.Repositories;
using Microsoft.Extensions.Logging;

namespace kube_consul_registrator.Services {
    public class ConsulRegisterCronJob : CronJobService 
    {
        //private readonly IKubernetesRepository _kubeRepo;
        private readonly ILogger<ConsulRegisterCronJob> _logger;
        private readonly IConsulRepository _consul;
        private ConsulServiceHelper _consulServiceHelper;
        private readonly IMapper _mapper;

        public ConsulRegisterCronJob(IScheduleConfig<ConsulRegisterCronJob> config, /*IKubernetesRepository kubeRepo,*/
            IConsulRepository consul, IMapper mapper, ILogger<ConsulRegisterCronJob> logger) : base(config.CronExpression, config.TimeZoneInfo) 
        {
            _mapper = mapper;
            _consul = consul;
            //_kubeRepo = kubeRepo;
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

            var consulServices = await _consul.GetServices();
            _consulServiceHelper = new ConsulServiceHelper(consulServices);

            // Get Kubernetes Pod information
            // Check annotation for register
            // Check the information for registratoin

            // Consul registration
            var consulRegistrationDto = new ConsulRegistrationDto 
            {
                ID = "testService",
                Name = "testService",
                Address = "localhost",
                Port = 55555
            };

            RegisterConsulService(consulRegistrationDto);
            DeregisterConsulService(consulRegistrationDto.ID);

        }

        public override Task StopAsync(CancellationToken cancellationToken) 
        {
            _logger.LogInformation("Consul register job is stopping");
            return base.StopAsync(cancellationToken);
        }

        private async void RegisterConsulService(ConsulRegistrationDto consulRegistrationDto) 
        {
            var registration = _mapper.Map<AgentServiceRegistration>(consulRegistrationDto);

            if (!_consulServiceHelper.CheckExistService(registration.ID)) 
            {
                var result = await _consul.RegisterService(registration);
                if (result == HttpStatusCode.OK) 
                {
                    _logger.LogInformation($"Result : {result.ToString()} - {registration.ID} service is resgistered.");
                } else
                {
                    _logger.LogError($"Result : {result.ToString()} - {registration.ID} service registration is Failed!!");
                } 
            }
        }

        private async void DeregisterConsulService(string id) 
        {
            if (_consulServiceHelper.CheckExistService(id)) {
                var result = await _consul.DeregisterService(id);
                if (result == HttpStatusCode.OK) 
                {
                    _logger.LogInformation ($"Result : {result.ToString()} - {id} service is deresgistered.");
                } else
                {
                    _logger.LogError($"Result : {result.ToString()} - {id} service deregistration is Failed!!");
                } 
            }
        }
    }
}