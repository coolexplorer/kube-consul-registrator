
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using kube_consul_registrator.Models;

namespace kube_consul_registrator.Repositories
{
    public interface IKubernetesRepository
    {
         Task<IEnumerable<PodInfo>> GetPods(string ns = "defualt");
    }
}