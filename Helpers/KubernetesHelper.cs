using System.Linq;
using System.Collections.Generic;
using kube_consul_registrator.Models;
using kube_consul_registrator.Const;

namespace kube_consul_registrator.Helpers
{
    public class KubernetesHelper
    {
        private readonly IEnumerable<PodInfo> _podInfo;
        
        public KubernetesHelper(IEnumerable<PodInfo> podInfo)
        {
            _podInfo = podInfo;
        }

        public List<PodInfo> GetConsulRegisterEnabledPods()
        {
            return _podInfo?.
                Where(p => p.Annotations != null && p.Annotations.Keys.Contains(Annotations.EABLED_ANNOTATION))?.
                Where(p => p.Annotations[Annotations.EABLED_ANNOTATION] == "true")?.
                ToList();
        }

        public List<PodInfo> GetConsulRegisterDisabledPods()
        {
            var disabledPds = _podInfo?.
                            Where(p => p.Annotations != null && p.Annotations.Keys.Contains(Annotations.EABLED_ANNOTATION))?.
                            Where(p => p.Annotations[Annotations.EABLED_ANNOTATION] == "false");

            var unAnnotatedPods = _podInfo?.
                            Where(p => p.Annotations == null || !p.Annotations.Keys.Contains(Annotations.EABLED_ANNOTATION));

            return disabledPds?.Concat(unAnnotatedPods)?.ToList();
        }
    }
}