using System.Collections.Generic;
using Consul;
using kube_consul_registrator.Helpers;
using Xunit;

namespace kube_consul_registrator.Tests
{
    public class ConsulServiceHelperTest
    {
        [Fact]
        public void GetRegisteredServiceIds_ReturnKeysList()
        {
            var consulHelper = new ConsulServiceHelper(GetTestConsulServices());

            var services = consulHelper.GetRegisteredServiceIds();

            Assert.Equal(2, services.Count);
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