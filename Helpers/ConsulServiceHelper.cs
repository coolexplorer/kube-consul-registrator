using System.Linq;
using System.Collections.Generic;
using Consul;
using kube_consul_registrator.Models;
using kube_consul_registrator.Dtos;
using kube_consul_registrator.Const;
using System;

namespace kube_consul_registrator.Helpers
{
    public class ConsulServiceHelper
    {
        private readonly IDictionary<string, AgentService> _consulServices;

        public ConsulServiceHelper(IDictionary<string, AgentService> consulServices)
        {
            _consulServices = consulServices;
        }
        public bool CheckExistService(string id)
        {
            if (_consulServices.Keys.Contains(id))
                return true;
            
            return false;
        }

        public List<string> GetRegisteredServiceIds()
        {
            return _consulServices.Keys.ToList();
        }

        public ConsulRegistrationDto CreateRegitration(PodInfo podInfo)
        {
            return new ConsulRegistrationDto
            {
                ID = podInfo.Annotations.Keys.Contains(Annotations.SERVICE_ID_ANNOTATION) ? podInfo.Annotations[Annotations.SERVICE_ID_ANNOTATION] : podInfo.Id,
                Name = podInfo.Annotations.Keys.Contains(Annotations.SERVICE_NAME_ANNOTATION) ? podInfo.Annotations[Annotations.SERVICE_NAME_ANNOTATION] : podInfo.Name,
                Address = podInfo.Ip,
                Port = podInfo.Annotations.Keys.Contains(Annotations.SERVICE_PORT_ANNOTATION) ? Convert.ToInt32(podInfo.Annotations[Annotations.SERVICE_PORT_ANNOTATION]) : Convert.ToInt32(podInfo.Port)
            };
        }
    }
}