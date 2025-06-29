using Microsoft.AspNetCore.Mvc;
using SaboresDoCerrado.ApiAutenticacao.Net.Service;
using System.Net;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Controller
{
    [ApiController]
    [Route("perfil")]
    public class PerfilController : ControllerBase
    {
        private readonly IPerfilService _perfilService;
        private readonly ILogger<PerfilController> _logger; //inject
        public PerfilController(IPerfilService perfilService, ILogger<PerfilController> logger)
        {
            _perfilService = perfilService;
            _logger = logger;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> GetPerfis()
        {
            _logger.LogInformation("Requisicao recebida - METHOD: GET, ENDPOINT: /perfil/listar");
            var perfis = await _perfilService.ObterTodosAsync();
            _logger.LogInformation("Requisicao finalizada - METHOD: GET, ENDPOINT: /perfil/listar, STATUS: {}", HttpStatusCode.OK.ToString());
            return Ok(perfis);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerfilPorId(int id)
        {
            _logger.LogInformation("METHOD: GET, ENDPOINT: /perfil/{}", id);
            var perfil = await _perfilService.ObterPorId(id);
            if (perfil == null)
            {
                _logger.LogWarning("Requisicao finalizada - METHOD: GET, ENDPOINT: /perfil/{}, STATUS: {}", id,HttpStatusCode.NotFound.ToString());
                return NotFound("Perfil não encontrado");
            }
            _logger.LogInformation("Requisicao finalizada - METHOD: GET, ENDPOINT: /perfil/{}, STATUS: {}", id, HttpStatusCode.OK.ToString());
            return Ok(perfil);
        }
    }
}
