using System.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Consul;
using kube_consul_registrator.Configurations;
using kube_consul_registrator.Dtos;
using kube_consul_registrator.Helpers;
using kube_consul_registrator.Models;
using kube_consul_registrator.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace kube_consul_registrator.Services
{
    public class ConsulRegisterCronJob : CronJobService
    {
        private readonly IKubernetesRepository _kubeRepo;
        private readonly ILogger<ConsulRegisterCronJob> _logger;
        private readonly IConsulRepository _consul;
        private ConsulServiceHelper _consulServiceHelper;
        private KubernetesHelper _kubernetesHelper;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        IEnumerable<PodInfo> _pods;

        public ConsulRegisterCronJob(IScheduleConfig<ConsulRegisterCronJob> scheduleConfig, IConfiguration config, IKubernetesRepository kubeRepo,
            IConsulRepository consul, IMapper mapper, ILogger<ConsulRegisterCronJob> logger) : base(scheduleConfig.CronExpression, scheduleConfig.TimeZoneInfo)
        {
            _config = config;
            _mapper = mapper;
            _consul = consul;
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

            var consulServices = await _consul.GetServices();

            _logger.LogInformation($"Got services from Consul");

            _logger.LogInformation($"Allowed namespace: {EnvironmentVariables.AllowedNamespaces}");

            var namespaces = EnvironmentVariables.AllowedNamespaces;
            
            foreach(string ns in namespaces)
            {
                var pods = await _kubeRepo.GetPods(ns);
                _pods = _pods.Concat(pods);
            } 

            _logger.LogInformation($"Got pods information from Kubernetes");

            _consulServiceHelper = new ConsulServiceHelper(consulServices);
            _kubernetesHelper = new KubernetesHelper(_pods);

            // Get Kubernetes Pod information
            // Check annotation for register
            // Pod has annotation && Not exist consul
            // Get Registration list
            var enabledPods = _kubernetesHelper.GetConsulRegisterEnabledPods();
            var registerCandidates = _consulServiceHelper.GetRegisterCandidates(enabledPods);


            // Check the information for registration
            // Exist consul && (No annotation || Not exist pod)
            // Get Deregistration list
            var disabledPods = _kubernetesHelper.GetConsulRegisterDisabledPods();
            var deRegisterCandidates = _consulServiceHelper.GetDeregisterCandidates(disabledPods);

            RegisterConsulService(registerCandidates);
            DeregisterConsulService(deRegisterCandidates);

        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consul register job is stopping");
            return base.StopAsync(cancellationToken);
        }

        private void RegisterConsulService(List<PodInfo> registerCandidates)
        {
            registerCandidates.ForEach(async pod => {
                var consulRegistrationDto = _consulServiceHelper.CreateRegitration(pod);

                var registration = _mapper.Map<AgentServiceRegistration>(consulRegistrationDto);

                if (!_consulServiceHelper.CheckExistService(registration.ID))
                {
                    var result = await _consul.RegisterService(registration);
                    if (result == HttpStatusCode.OK)
                    {
                        _logger.LogInformation($"Result : {result.ToString()} - {registration.ID} service is resgistered.");
                    }
                    else
                    {
                        _logger.LogError($"Result : {result.ToString()} - {registration.ID} service registration is Failed!!");
                    }
                }
            });
        }

        private void DeregisterConsulService(List<PodInfo> deRegisterCandidates)
        {
            deRegisterCandidates.ForEach(async pod => {
                var id = pod.Name;

                if (_consulServiceHelper.CheckExistService(id))
                {
                    var result = await _consul.DeregisterService(id);
                    if (result == HttpStatusCode.OK)
                    {
                        _logger.LogInformation($"Result : {result.ToString()} - {id} service is deresgistered.");
                    }
                    else
                    {
                        _logger.LogError($"Result : {result.ToString()} - {id} service deregistration is Failed!!");
                    }
                }
            });
        }
    }
}