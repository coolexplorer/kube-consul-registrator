using System.Collections.Generic;
using System.Linq;
using kube_consul_registrator.Const;
using kube_consul_registrator.Helpers;
using kube_consul_registrator.Models;
using Xunit;

namespace kube_consul_registrator.Tests
{
    public class KubernetesHelperTest
    {
        private readonly List<PodInfo> _podInfo;
        public KubernetesHelperTest()
        {
            _podInfo = GetTestPods();
        }

        [Fact]
        public void GetConsulRegisterEnabledPods_ReturnPodInfoList()
        {
            var kubernetesHelper = new KubernetesHelper(_podInfo);

            var enabledPods = kubernetesHelper.GetConsulRegisterEnabledPods();

            var result = Assert.IsType<List<PodInfo>>(enabledPods);
            Assert.Equal(_podInfo.Take(2).Select(p => p.Name).ToList(), enabledPods.Select(p => p.Name).ToList());
        }

        [Fact]
        public void GetConsulRegisterDisabledPods_ReturnPodInfoList()
        {
            var kubernetesHelper = new KubernetesHelper(_podInfo);

            var disabledPods = kubernetesHelper.GetConsulRegisterDisabledPods();

            var result = Assert.IsType<List<PodInfo>>(disabledPods);
            Assert.Equal(_podInfo.Last().Name, disabledPods.FirstOrDefault().Name);
        }

        private List<PodInfo> GetTestPods()
        {
            return new List<PodInfo>()
            {
                new PodInfo()
                {
                    Name = "pushgateway",
                    NodeName = "node1",
                    Ip = "127.0.0.1",
                    Containers = null,
                    Phase = PodPhase.RUNNING,
                    Annotations = new Dictionary<string, string>()
                    {
                        {Annotations.EABLED_ANNOTATION, "true"},
                        {Annotations.SERVICE_ID_ANNOTATION, "pushgateway"},
                        {Annotations.SERVICE_NAME_ANNOTATION, "pushgateway"},
                        {Annotations.SERVICE_METADATA_ANNOTATION + "test", "test"}
                    }
                },
                new PodInfo()
                {
                    Name = "auth",
                    NodeName = "node1",
                    Ip = "127.0.0.1",
                    Containers = null,
                    Phase = PodPhase.PENDING,
                    Annotations = new Dictionary<string, string>()
                    {
                        {Annotations.EABLED_ANNOTATION, "true"},
                        {Annotations.SERVICE_ID_ANNOTATION, "auth"},
                        {Annotations.SERVICE_NAME_ANNOTATION, "auth"},
                        {Annotations.SERVICE_PORT_ANNOTATION, "80"},
                        {Annotations.SERVICE_METADATA_ANNOTATION + "test", "test"}
                    }
                },
                new PodInfo()
                {
                    Name = "consul",
                    NodeName = "node2",
                    Ip = "127.0.0.1",
                    Containers = null,
                    Phase = PodPhase.PENDING,
                    Annotations = new Dictionary<string, string>(){}
                }
            };
        }
    }
}