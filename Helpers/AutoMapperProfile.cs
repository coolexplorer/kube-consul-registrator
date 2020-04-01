using AutoMapper;
using Consul;
using kube_consul_registrator.Dtos;

namespace kube_consul_registrator.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ConsulRegistrationDto, AgentServiceRegistration>();    
        }
    }
}