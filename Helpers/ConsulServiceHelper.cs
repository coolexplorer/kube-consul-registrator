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

        public List<PodInfo> GetRegisterCandidates(List<PodInfo> enabledPods)
        {
            return enabledPods.Where(p => !_consulServices.Keys.Contains(p.Name)).ToList();
        }

        public List<string> GetDeregisterCandidates(List<PodInfo> disabledPods)
        {
            return disabledPods.Where(p => _consulServices.Keys.Contains(p.Name)).Select(p => p.Name).ToList();
        }

        public List<string> GetDeletedPods(List<PodInfo> wholePods)
        {
            var podIds = wholePods.Select(p => p.Name).ToList();

            return _consulServices?.Keys.Where(k => !podIds.Contains(k)).ToList();
        }

        public ConsulRegistrationDto CreateRegitration(PodInfo podInfo)
        {
            string serviceId = null, serviceName = null;
            int servicePort = 0;

            if (podInfo.Annotations.Keys.Contains(Annotations.SERVICE_ID_ANNOTATION))
            {
                serviceId = podInfo.Annotations[Annotations.SERVICE_ID_ANNOTATION];
            }
            else
            {
                serviceId = podInfo.Name;
            }

            if (podInfo.Annotations.Keys.Contains(Annotations.SERVICE_NAME_ANNOTATION))
            {
                serviceName = podInfo.Annotations[Annotations.SERVICE_NAME_ANNOTATION];
            }
            else
            {
                serviceName = podInfo.Name;
            }

            if (podInfo.Annotations.Keys.Contains(Annotations.SERVICE_PORT_ANNOTATION))
            {
                servicePort = Convert.ToInt32(podInfo.Annotations[Annotations.SERVICE_PORT_ANNOTATION]);
            }
            else
            {
                var container = podInfo.Containers.Where(c => c.Ports != null).FirstOrDefault();

                if (container.Ports != null)
                {
                    servicePort = container.Ports.Select(p => p.ContainerPort).FirstOrDefault();
                }
                else
                {
                    servicePort = 80;
                }
            }

            return new ConsulRegistrationDto
            {
                ID =  serviceId,
                Name = serviceName,
                Address = podInfo.Ip,
                Port = servicePort,
                Meta = ParseServiceMetaData(podInfo)
            };
        }

        public IDictionary<string, string> ParseServiceMetaData(PodInfo pod)
        {
            Dictionary<string, string> meta = new Dictionary<string, string>();

            if (ExistMetaAnnotation(pod))
            {
                pod.Annotations.Keys
                .Where(k => k.Contains(Annotations.SERVICE_METADATA_ANNOTATION)).ToList()
                .ForEach(k => {
                    meta.Add(k.Replace(Annotations.SERVICE_METADATA_ANNOTATION, ""), pod.Annotations[k]);
                });
            }
            
            return meta;
        }

        public bool ExistMetaAnnotation(PodInfo pod)
        {
            foreach(string key in pod.Annotations.Keys)
            {
                if (key.Contains(Annotations.SERVICE_METADATA_ANNOTATION))
                {
                    return true;
                }
            }

            return false;
        }
    }
}