using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Consul;

namespace kube_consul_registrator.Repositories
{
    public interface IConsulRepository
    {
        Task<IDictionary<string, AgentService>> GetServices();
        Task<AgentService> GetService(string id);
        Task<HttpStatusCode> RegisterService(AgentServiceRegistration registration);
        Task<HttpStatusCode> DeregisterService(string id);
    }
}