using System.Collections.Generic;
using Consul;

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
    }
}