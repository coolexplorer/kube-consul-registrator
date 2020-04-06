using System.Linq;
using System.Threading.Tasks;
using k8s;
using kube_consul_registrator.Models;
using System.Collections.Generic;

namespace kube_consul_registrator.Repositories
{
    public class KubernetesRepository : IKubernetesRepository
    {
        private readonly Kubernetes _client;
        public KubernetesRepository()
        {
            _client = new Kubernetes(KubernetesClientConfiguration.InClusterConfig());
        }

        public async Task<IEnumerable<PodInfo>> GetPods(string ns = "default")
        {
            var pods = await _client.ListNamespacedPodAsync(ns);

            return pods.Items.Select(pods =>
                new PodInfo
                {
                    Name = pods.Metadata.Name,
                    NodeName = pods.Spec.NodeName,
                    Ip = pods.Status.PodIP,
                    Phase = pods.Status.Phase,
                    Port = pods.Spec.Containers.Select(c => c.Ports).FirstOrDefault().Select(c =>  c.ContainerPort).FirstOrDefault(),
                    Annotations = pods.Metadata.Annotations
                });
        }
    }
}