using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace kube_consul_registrator.Dtos
{
    [ExcludeFromCodeCoverage]
    public class ConsulRegistrationDto
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string[] Tags { get; set; } = null;
        public IDictionary<string, string> Meta { get; set; }
    }
}