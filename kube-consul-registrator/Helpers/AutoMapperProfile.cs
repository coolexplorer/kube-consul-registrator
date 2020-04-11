using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Consul;
using kube_consul_registrator.Dtos;

namespace kube_consul_registrator.Helpers
{
    [ExcludeFromCodeCoverage]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ConsulRegistrationDto, AgentServiceRegistration>();    
        }
    }
}