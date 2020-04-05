using System.Collections.Generic;

namespace kube_consul_registrator.Models
{
    public class PodInfo
    {
        public string Name { get; set; }
        public string NodeName { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Phase { get; set; }
        public IDictionary<string, string> Annotations { get; set; }
    }
}