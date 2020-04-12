using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using kube_consul_registrator.Repositories;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace kube_consul_registrator.Tests
{
    public class ConsulRepositoryTest
    {
        Mock<IConsulClient> _mockConsul;
        ConsulRepository _consulRepo;
        public ConsulRepositoryTest()
        {
            _mockConsul = new Mock<IConsulClient>(); 
            _consulRepo = new ConsulRepository(_mockConsul.Object);
        }

        [Fact]
        public void GetServices_ReturnAgentServices()
        {
            _mockConsul.Setup(c => c.Agent.Services(It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetServices()));

            var task = _consulRepo.GetServices();
            task.Wait();

            var expectedMap = new Dictionary<string, AgentService>()
            {
                {
                    "consul", new AgentService(){
                        ID = "consul",
                        Service = "consul",
                        Tags = new string[]{},
                        Port = 80,
                        Address = "127.0.0.1",
                        EnableTagOverride = false,
                        Meta = new Dictionary<string, string>(){
                            {"MajorVersion", "1"}
                        }
                    }
                }
            };

            var result = Assert.IsType<Dictionary<string, AgentService>>(task.Result);
            
            var obj1Str = JsonConvert.SerializeObject(expectedMap);
            var obj2Str = JsonConvert.SerializeObject(result);

            Assert.True(obj1Str.Equals(obj2Str));
        }

        [Fact]
        public void GetService_ReturnAgentService()
        {
            _mockConsul.Setup(c => c.Agent.Services(It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetServices()));

            var task = _consulRepo.GetService("consul");
            task.Wait();

            var expectedService = new AgentService(){
                        ID = "consul",
                        Service = "consul",
                        Tags = new string[]{},
                        Port = 80,
                        Address = "127.0.0.1",
                        EnableTagOverride = false,
                        Meta = new Dictionary<string, string>(){
                            {"MajorVersion", "1"}
                        }
            };

            var result = Assert.IsType<AgentService>(task.Result);
            
            var obj1Str = JsonConvert.SerializeObject(expectedService);
            var obj2Str = JsonConvert.SerializeObject(result);

            Assert.True(obj1Str.Equals(obj2Str));
        }

        [Fact]
        public void RegisterService_ReturnHttpStatusCode()
        {
            _mockConsul.Setup(c => c.Agent.ServiceRegister(It.IsAny<AgentServiceRegistration>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetWriteResult()));

            var registration = new AgentServiceRegistration()
            {
                ID = "consul",
                Name = "Name",
                Tags = new string[]{},
                Port = 80,
                Address = "127.0.0.1",
                EnableTagOverride = false,
                Meta = new Dictionary<string, string>(){}
            };

            var task = _consulRepo.RegisterService(registration);
            task.Wait();

            var result = Assert.IsType<HttpStatusCode>(task.Result);

            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public void DeregisterService_ReturnHttpStatusCode()
        {
            _mockConsul.Setup(c => c.Agent.ServiceDeregister(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetWriteResult()));


            var task = _consulRepo.DeregisterService("consul");
            task.Wait();

            var result = Assert.IsType<HttpStatusCode>(task.Result);

            Assert.Equal(HttpStatusCode.OK, result);
        }

        private QueryResult<Dictionary<string, AgentService>> GetServices()
        {
            var queryResult = new QueryResult<Dictionary<string, AgentService>>();
            queryResult.Response = GetTestServices();

            return queryResult;
        }

        private Dictionary<string, AgentService> GetTestServices()
        {
            return new Dictionary<string, AgentService>()
            {
                {
                    "consul", new AgentService(){
                        ID = "consul",
                        Service = "consul",
                        Tags = new string[]{},
                        Port = 80,
                        Address = "127.0.0.1",
                        EnableTagOverride = false,
                        Meta = new Dictionary<string, string>(){
                            {"MajorVersion", "1"}
                        }
                    }
                }
            };
        }

        private WriteResult GetWriteResult()
        {
            return new WriteResult()
            {
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}