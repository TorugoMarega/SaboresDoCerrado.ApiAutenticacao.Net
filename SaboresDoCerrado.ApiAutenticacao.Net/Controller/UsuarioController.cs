using Microsoft.AspNetCore.Mvc;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Service;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Controller
{
    [ApiController]
    [Route("usuario")]
    public class UsuarioController:ControllerBase
    {
        private readonly IUsuarioService _userService;

        public UsuarioController (IUsuarioService userService)
        {
            _userService = userService;
        }

        [HttpGet("listar")]
        public async Task<IEnumerable<UsuarioDTO>> ObterTodosUsuarios() {
            return await _userService.ObterTodosAsync();
        }
    }
}
