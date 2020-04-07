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
            _logger.LogDebug($"Gethering pod information from kubernetes");

            var consulServices = await _consul.GetServices();

             _logger.LogDebug("consulServices: [{0}]", string.Join(",", consulServices.Select(s => s.Key).ToArray()));

            var namespaces = EnvironmentVariables.AllowedNamespaces;

            IEnumerable<PodInfo> wholePods = null;
            
            foreach(string ns in namespaces)
            {
                _logger.LogDebug($"Get Pods from {ns}");

                var pods = await _kubeRepo.GetPods(ns);
                
                if (pods != null)
                {
                    if (wholePods == null)
                    {
                        wholePods = pods;
                    }
                    else
                    {
                        wholePods = wholePods.Concat(pods);
                    }
                }

                _logger.LogDebug("Pods: [{0}]", string.Join(",", wholePods.Select(p => p.Name).ToArray()));
            }

            _logger.LogDebug($"Got pods information from Kubernetes");

            _consulServiceHelper = new ConsulServiceHelper(consulServices);
            _kubernetesHelper = new KubernetesHelper(wholePods);

            var enabledPods = _kubernetesHelper.GetConsulRegisterEnabledPods();

            _logger.LogDebug("enabledPods: [{0}]", string.Join(",", enabledPods.Select(p => p.Name).ToArray()));

            var registerCandidates = _consulServiceHelper.GetRegisterCandidates(enabledPods);

            _logger.LogDebug("registerCandidates: [{0}]", string.Join(",", registerCandidates.Select(p => p.Name).ToArray()));

            var disabledPods = _kubernetesHelper.GetConsulRegisterDisabledPods();

            _logger.LogDebug("disabledPods: [{0}]", string.Join(",", disabledPods.Select(p => p.Name).ToArray()));

            var deRegisterCandidates = _consulServiceHelper.GetDeregisterCandidates(disabledPods);

            _logger.LogDebug("deRegisterCandidates: [{0}]", string.Join(",", deRegisterCandidates));

            var deletedPods = _consulServiceHelper.GetDeletedPods(wholePods.ToList());

            _logger.LogDebug("deletedPods: [{0}]", string.Join(",", deletedPods));

            if (registerCandidates != null)
            {
                RegisterConsulService(registerCandidates);
            }

            if (deRegisterCandidates != null)
            {
                DeregisterConsulService(deRegisterCandidates);
            }

            if (deletedPods != null)
            {
                DeregisterConsulService(deletedPods);
            }
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

        private void DeregisterConsulService(List<string> deRegisterCandidates)
        {
            deRegisterCandidates.ForEach(async id => {
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