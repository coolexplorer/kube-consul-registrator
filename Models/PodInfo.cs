using System.Collections.Generic;

namespace kube_consul_registrator.Models
{
    public class PodInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NodeName { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public IDictionary<string, string> Annotations { get; set; }
    }
}