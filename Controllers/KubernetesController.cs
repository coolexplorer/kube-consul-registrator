using System;
using System.Threading.Tasks;
using kube_consul_registrator.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace kube_consul_registrator.Controllers {
    [ApiController]
    [Route ("api/[Controller]")]
    public class KubernetesController : ControllerBase {
        private readonly IKubernetesRepository _kubeRepo;
        private readonly ILogger<KubernetesController> _logger;
        public KubernetesController (IKubernetesRepository kubeRepo, ILogger<KubernetesController> logger) {
            _logger = logger;
            _kubeRepo = kubeRepo;
        }

        [HttpGet("pods/{ns}")]
        public async Task<IActionResult> GetPods(string ns = "default")
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var pods = await _kubeRepo.GetPods(ns);
                return Ok(pods);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode(500);
            }
        }
    }
}