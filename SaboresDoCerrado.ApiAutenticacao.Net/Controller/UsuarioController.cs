using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using SaboresDoCerrado.ApiAutenticacao.Net.Service;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObterTodosUsuarios()
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para listar todos os usuários");
            var usuarios = await _userService.ObterTodosAsync();
            _logger.LogInformation(
               "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
               HttpContext.Request.Method,
               HttpContext.Request.Path,
               200,
               stopwatch.ElapsedMilliseconds
            );
            return Ok(usuarios);
        }

        [HttpGet("{id}", Name = "ObterPorId")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para inativar usuário ID: [{UsuarioId}]", id);

            var usuario = await _userService.ObterPorIdNoTrackAsync(id);
            if (usuario is null)
            {
                stopwatch.Stop();
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
            stopwatch.Stop();
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
                stopwatch.Stop();
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
        public async Task<IActionResult> UpdateUsuarioPorId(int id, [FromBody] UsuarioUpdateRequestDTO usuarioUpdateRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para atualizar usuário ID: [{UsuarioId}]", id);

            try
            {
                var usuarioAtualizado = await _userService.UpdateUsuarioPorId(id, usuarioUpdateRequestDTO);

                if (usuarioAtualizado is null)
                {
                    stopwatch.Stop();
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
                stopwatch.Stop();
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
        [HttpPut("senha/alterar")]
        [Authorize]
        public async Task<IActionResult> UpdateSenhaUsuarioPorId([FromBody] UsuarioUpdateSenhaRequestDTO usuarioUpdateRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.LogDebug("Buscando ID a partir do Header da requisicao");
                var idUsuarioLogadoString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogDebug("String ID Usuario: [{id}]", idUsuarioLogadoString);
                if (idUsuarioLogadoString is null)
                {
                    {
                        stopwatch.Stop();
                        var msg = "Token expirado ou inválido";
                        _logger.LogWarning(
                        "{msg}. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                        msg,
                        HttpContext.Request.Method,
                        HttpContext.Request.Path,
                        401,
                        stopwatch.ElapsedMilliseconds
                        );
                        return Unauthorized(new { statusCode = 401, message = msg });
                    }
                }
                _logger.LogInformation("Requisição recebida para atualizar senha do usuário ID: [{UsuarioId}]", idUsuarioLogadoString);
                var usuarioAtualizado = await _userService.UpdateSenhaUsuarioPorIdAsync(int.Parse(idUsuarioLogadoString), usuarioUpdateRequestDTO);

                if (usuarioAtualizado.Equals(false))
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                    "Tentativa de atualizar senha de um usuário não existente ID: [{UsuarioId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    idUsuarioLogadoString,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    404,
                    stopwatch.ElapsedMilliseconds
                    );
                    return NotFound();
                }

                stopwatch.Stop();
                _logger.LogInformation(
                    "Senha do usuário atualizado com sucesso: [{UsuarioId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    idUsuarioLogadoString,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    200,
                    stopwatch.ElapsedMilliseconds
                    );
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                stopwatch.Stop();
                _logger.LogInformation(
               "{msg}. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
               ex.Message,
               HttpContext.Request.Method,
               HttpContext.Request.Path,
               400,
               stopwatch.ElapsedMilliseconds
               );
                return BadRequest(new { statusCode = 400, message = ex.Message });
            }
        }
    }
}
