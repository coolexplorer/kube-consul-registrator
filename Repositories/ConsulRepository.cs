using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;

namespace kube_consul_registrator.Repositories
{
    public class ConsulRepository : IConsulRepository 
    {
        private readonly IConsulClient _consul;
        public ConsulRepository (IConsulClient consul) 
        {
            _consul = consul;
        }

        public async Task<AgentService> GetService(string id)
        {
            var services = await _consul.Agent.Services();
            
            return services.Response.Where(s => s.Key == id).FirstOrDefault().Value;
        }

        public async Task<IDictionary<string, AgentService>> GetServices() 
        {
            var services = await _consul.Agent.Services();
            
            return services.Response;
        }

        public async Task<HttpStatusCode> RegisterService(AgentServiceRegistration registration)
        {
            var result = await _consul.Agent.ServiceRegister(registration);

            return result.StatusCode;
        }

        public async Task<HttpStatusCode> DeregisterService(string id)
        {
            var result = await _consul.Agent.ServiceDeregister(id);

            return result.StatusCode;
        }

    }
}