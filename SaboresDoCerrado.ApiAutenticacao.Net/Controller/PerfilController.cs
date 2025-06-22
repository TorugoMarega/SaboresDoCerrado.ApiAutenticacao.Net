using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SaboresDoCerrado.ApiAutenticacao.Net.Service;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Controller
{
    [ApiController]
    [Route("perfil")]
    public class PerfilController : ControllerBase
    {
        private readonly IPerfilService _perfilService;
        public PerfilController (IPerfilService perfilService)
        {
            _perfilService = perfilService;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> GetPerfis()
        {
            var perfis = await _perfilService.ObterTodosAsync();

            return Ok(perfis);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerfilPorId(int id)
        {
            var perfil = await _perfilService.ObterPorId(id);

            return Ok(perfil);
        }
        
    }
}
