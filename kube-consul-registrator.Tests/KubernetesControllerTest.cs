using System.Collections.Generic;
using Xunit;
using kube_consul_registrator.Models;
using kube_consul_registrator.Repositories;
using kube_consul_registrator.Controllers;
using kube_consul_registrator.Const;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace kube_consul_registrator.Tests
{
    public class KubernetesControllerTest
    {
        [Fact]
        public void GetPods_BadRequestResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var mockReop = new Mock<IKubernetesRepository>();
            var mockLogger = new Mock<ILogger<KubernetesController>>();
            var controller = new KubernetesController(mockReop.Object, mockLogger.Object);
            controller.ModelState.AddModelError("Name", "Required");

            // Act
            var task = controller.GetPods("default");

            task.Wait();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(task.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void GetPods_OkResult_WhenModelStateIsValid()
        {
            // Arrage
            var mockRepo = new Mock<IKubernetesRepository>();
            var mockLogger = new Mock<ILogger<KubernetesController>>();
            mockRepo.Setup(repo => repo.GetPods("default")).Returns(Task.FromResult(GetTestPods()));
            var controller = new KubernetesController(mockRepo.Object, mockLogger.Object);

            // Act
            var task = controller.GetPods("default");
            task.Wait();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(task.Result);
            var model = Assert.IsAssignableFrom<List<PodInfo>>(okResult.Value);
            Assert.Equal(2, model.Count);
        }

        private IEnumerable<PodInfo> GetTestPods()
        {
            return new List<PodInfo>()
            {
                new PodInfo()
                {
                    Name = "consul",
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
                    Name = "auth",
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
    }
}
