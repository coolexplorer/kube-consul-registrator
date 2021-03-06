using System.Linq;
using System.Threading.Tasks;
using k8s;
using kube_consul_registrator.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace kube_consul_registrator.Repositories
{
    [ExcludeFromCodeCoverage]
    public class KubernetesRepository : IKubernetesRepository
    {
        private readonly IKubernetes _client;
        private readonly ILogger<KubernetesRepository> _logger;
        public KubernetesRepository(IKubernetes client, ILogger<KubernetesRepository> logger)
        {
            _client = client;
            _logger = logger;
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