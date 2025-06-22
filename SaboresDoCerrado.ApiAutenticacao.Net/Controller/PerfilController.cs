using Microsoft.AspNetCore.Mvc;
using SaboresDoCerrado.ApiAutenticacao.Net.Service;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Controller
{
    [ApiController]
    [Route("/perfil/listar")]
    public class PerfilController : ControllerBase
    {
        private readonly IPerfilService _perfilService;

        public PerfilController (IPerfilService perfilService)
        {
            _perfilService = perfilService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetPerfis()
        {
            var perfis = await _perfilService.ObterTodosAsync();

            return Ok(perfis);
        }
        // TODO
        /*
        [HttpGet]
        public async Task<IActionResult> GetPerfilPorId()
        {
            var perfis = await _perfilService.ObterTodosAsync();

            return Ok(perfis);
        }
        */
    }
}
