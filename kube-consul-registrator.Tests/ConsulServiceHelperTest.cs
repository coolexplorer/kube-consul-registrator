using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Consul;
using kube_consul_registrator.Const;
using kube_consul_registrator.Helpers;
using kube_consul_registrator.Models;
using Xunit;
using kube_consul_registrator.Dtos;
using Newtonsoft.Json;

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
            var cadidates = _consulHelper.GetRegisterCandidates(GetTestPods());

            var result = Assert.IsType<List<PodInfo>>(cadidates);
            Assert.Single(result);
        }

        [Fact]
        public void GetDeregisterCandidates_ReturnPodInfoList()
        {
            var cadidates = _consulHelper.GetDeregisterCandidates(GetTestPods());

            var result = Assert.IsType<List<string>>(cadidates);
            Assert.Single(result);
        }

        [Fact]
        public void GetDeletedPods_ReturnPodInfoList()
        {
            var cadidates = _consulHelper.GetDeletedPods(GetTestPods(), null);

            var result = Assert.IsType<List<string>>(cadidates);
            Assert.Single(result);
        }

        [Fact]
        public void CreateRegitration_ReturnConsulRegistrationDto1()
        {
            var registrationDto = _consulHelper.CreateRegitration(GetTestPods().First());

            var result = Assert.IsType<ConsulRegistrationDto>(registrationDto);

            var expectedDto = new ConsulRegistrationDto()
                            {
                                ID = "pushgateway",
                                Name = "pushgateway",
                                Address = "127.0.0.1",
                                Port = 80,
                                Tags = new string[]{},
                                Meta = new Dictionary<string, string>()
                                {
                                    {"test", "test"}
                                }
                            };

            var obj1Str = JsonConvert.SerializeObject(expectedDto);
            var obj2Str = JsonConvert.SerializeObject(result);

            Assert.True(obj1Str.Equals(obj2Str));
        }

        [Fact]
        public void CreateRegitration_ReturnConsulRegistrationDto2()
        {
            var registrationDto = _consulHelper.CreateRegitration(GetTestPods()[1]);

            var result = Assert.IsType<ConsulRegistrationDto>(registrationDto);

            var expectedDto = new ConsulRegistrationDto()
                            {
                                ID = "auth",
                                Name = "auth",
                                Address = "127.0.0.1",
                                Port = 80,
                                Tags = new string[]{"tag1", "tag2"},
                                Meta = new Dictionary<string, string>()
                                {
                                    {"test", "test"}
                                }
                            };

            var obj1Str = JsonConvert.SerializeObject(expectedDto);
            var obj2Str = JsonConvert.SerializeObject(result);

            Assert.True(obj1Str.Equals(obj2Str));
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
                        {Annotations.EABLED, "true"},
                        {Annotations.SERVICE_ID, "pushgateway"},
                        {Annotations.SERVICE_NAME, "pushgateway"},
                        {Annotations.SERVICE_METADATA + "test", "test"}
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
                        {Annotations.EABLED, "true"},
                        {Annotations.SERVICE_ID, "auth"},
                        {Annotations.SERVICE_NAME, "auth"},
                        {Annotations.SERVICE_PORT, "80"},
                        {Annotations.SERVICE_TAG, "tag1,tag2"},
                        {Annotations.SERVICE_METADATA + "test", "test"}
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
                        {Annotations.EABLED, "true"},
                    }
                }
            };
        }

        private List<PodInfo> GetTestRegisterCandidatePods()
        {
            return new List<PodInfo>()
            {
                new PodInfo()
                {
                    Name = "report",
                    NodeName = "node1",
                    Ip = "127.0.0.1",
                    Containers = null,
                    Phase = PodPhase.RUNNING,
                    Annotations = new Dictionary<string, string>()
                    {
                        {Annotations.EABLED, "true"},
                        {Annotations.SERVICE_ID, "pushgateway"},
                        {Annotations.SERVICE_NAME, "pushgateway"},
                        {Annotations.SERVICE_METADATA + "test", "test"}
                    }
                }            };
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