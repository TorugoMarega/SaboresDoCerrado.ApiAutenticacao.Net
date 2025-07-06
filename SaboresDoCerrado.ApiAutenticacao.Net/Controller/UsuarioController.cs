using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request;
using SaboresDoCerrado.ApiAutenticacao.Net.Service;
using System.Diagnostics;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Controller
{
    [ApiController]
    [Route("usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _userService;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(IUsuarioService userService, ILogger<UsuarioController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ObterTodosUsuarios()
        {
            var usuarios = await _userService.ObterTodosAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para inativar usuário ID: [{UsuarioId}]", id);

                var usuario = await _userService.ObterPorIdAsync(id);
                stopwatch.Stop();
                if (usuario is null)
                {
                    _logger.LogWarning(
                   "Usuário não encontrado na base: [{id}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                   id,
                   HttpContext.Request.Method,
                   HttpContext.Request.Path,
                   404,
                   stopwatch.ElapsedMilliseconds
                   );
                    return NotFound();
                }
                _logger.LogInformation(
                    "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    200,
                    stopwatch.ElapsedMilliseconds
                    );
                return Ok(usuario);
        }
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> InativarPorId(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para INATIVAR usuário ID: [{UsuarioId}]", id);

            var sucesso = await _userService.InativarAtivarUsuarioAsync(id, false);

            if (!sucesso)
            {
                _logger.LogWarning(
                    "Tentativa de inativar usuário não existente ID: [{UsuarioId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    id,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    404,
                    stopwatch.ElapsedMilliseconds
                    );
                return NotFound();
            }

            stopwatch.Stop();
            _logger.LogInformation(
                "Usuário inativado com sucesso: [{UsuarioId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                id,
                HttpContext.Request.Method,
                HttpContext.Request.Path,
                200,
                stopwatch.ElapsedMilliseconds
                );
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuarioPorId(int id, [FromBody] UsuarioUpdateRequestDTO usuarioUpdateRequestDTO) {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para atualizar usuário ID: [{UsuarioId}]", id);

            try
            {
                var usuarioAtualizado = await _userService.UpdateUsuarioPorId(id, usuarioUpdateRequestDTO);

                if (usuarioAtualizado is null)
                {
                    _logger.LogWarning(
                    "Tentativa de atualizar usuário não existente ID: [{UsuarioId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    id,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    404,
                    stopwatch.ElapsedMilliseconds
                    );
                    return NotFound();
                }

                stopwatch.Stop();
                _logger.LogInformation(
                    "Usuário atualizado com sucesso: [{UsuarioId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    id,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    200,
                    stopwatch.ElapsedMilliseconds
                    );
                return Ok(usuarioAtualizado);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(
               "{msg}. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
               ex.Message,
               HttpContext.Request.Method,
               HttpContext.Request.Path,
               400,
               stopwatch.ElapsedMilliseconds
               );
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
