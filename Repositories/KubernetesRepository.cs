using System.ComponentModel;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using kube_consul_registrator.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace kube_consul_registrator.Repositories
{
    public class KubernetesRepository : IKubernetesRepository
    {
        private readonly Kubernetes _client;
        private readonly ILogger<KubernetesRepository> _logger;
        public KubernetesRepository(ILogger<KubernetesRepository> logger)
        {
            _logger = logger;
            _client = new Kubernetes(KubernetesClientConfiguration.InClusterConfig());
        }

        public async Task<IEnumerable<PodInfo>> GetPods(string ns = "default")
        {
            var pods = await _client.ListNamespacedPodAsync(ns);

            _logger.LogDebug("pods information: [{0}]", string.Join(",", pods?.Items?.Select(p => p.Metadata.Name).ToArray()));

            return pods?.Items?.Select(pod =>
                new PodInfo
                {
                    Name = pod.Metadata.Name,
                    NodeName = pod.Spec.NodeName,
                    Ip = pod.Status.PodIP,
                    Phase = pod.Status.Phase,
                    Containers = pod.Spec.Containers,
                    Annotations = pod.Metadata.Annotations
                });
        }
    }
}