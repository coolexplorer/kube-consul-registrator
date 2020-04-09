using System.Collections.Generic;
using System.Threading.Tasks;
using Consul;
using kube_consul_registrator.Const;
using kube_consul_registrator.Helpers;
using kube_consul_registrator.Models;
using Xunit;

namespace kube_consul_registrator.Tests
{
    public class ConsulServiceHelperTest
    {
        private readonly ConsulServiceHelper _consulHelper;
        public ConsulServiceHelperTest()
        {
            _consulHelper = new ConsulServiceHelper(GetTestConsulServices());
        }

        [Fact]
        public void GetRegisteredServiceIds_ReturnKeysList()
        {
            var services = _consulHelper.GetRegisteredServiceIds();

            Assert.Equal(2, services.Count);
        }

        [Fact]
        public void GetRegisterCandidates_ReturnPodInfoList()
        {
            var cadidates = _consulHelper.GetRegisterCandidates(GetTestEnabledPods());

            var result = Assert.IsType<List<PodInfo>>(cadidates);
            Assert.Single(result);
        }

        private List<PodInfo> GetTestEnabledPods()
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
                        {"MajorVersion", "1"}
                    }
                },
                new PodInfo()
                {
                    Name = "consul",
                    NodeName = "node2",
                    Ip = "127.0.0.1",
                    Containers = null,
                    Phase = PodPhase.PENDING,
                    Annotations = new Dictionary<string, string>()
                    {
                        {"MajorVersion", "0"}
                    }
                }
            };
        }

        private IDictionary<string, AgentService> GetTestConsulServices()
        {
            return new Dictionary<string, AgentService>()
            {
                {"consul", new AgentService()
                            {
                                ID = "consul",
                                Service = "consul",
                                Tags = new string[] {"tag"},
                                Port = 8500,
                                Address = "127.0.0.1",
                                EnableTagOverride = false,
                                Meta = new Dictionary<string, string>()
                                {
                                    {"name", "consul"}
                                }
                            }
                },
                {"nginx", new AgentService()
                            {
                                ID = "nginx",
                                Service = "nginx",
                                Tags = new string[] {"tag2"},
                                Port = 80,
                                Address = "127.0.0.1",
                                EnableTagOverride = false,
                                Meta = new Dictionary<string, string>()
                                {
                                    {"name", "nginx"}
                                }
                            }
                }
            };
        }
    }
}