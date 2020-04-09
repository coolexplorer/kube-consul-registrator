using System.Collections.Generic;

namespace kube_consul_registrator.Dtos
{
    public class ConsulRegistrationDto
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string[] tags { get; set; } = null;
        public IDictionary<string, string> Meta { get; set; }
    }
}