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
                Where(p => p.Annotations != null && p.Annotations.Keys.Contains(Annotations.EABLED))?.
                Where(p => p.Annotations[Annotations.EABLED] == "true")?.
                ToList();
        }

        public List<PodInfo> GetConsulRegisterDisabledPods()
        {
            var disabledPds = _podInfo?.
                            Where(p => p.Annotations != null && p.Annotations.Keys.Contains(Annotations.EABLED))?.
                            Where(p => p.Annotations[Annotations.EABLED] == "false");

            var unAnnotatedPods = _podInfo?.
                            Where(p => p.Annotations == null || !p.Annotations.Keys.Contains(Annotations.EABLED));

            return disabledPds?.Concat(unAnnotatedPods)?.ToList();
        }
    }
}