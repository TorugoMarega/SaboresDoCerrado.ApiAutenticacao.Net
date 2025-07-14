using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using SaboresDoCerrado.ApiAutenticacao.Net.Service;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Controller
{
    [ApiController]
    [Route("usuario")]
    [Authorize]
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
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObterTodosUsuariosAsync()
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
        public async Task<IActionResult> ObterUsuarioPorIdAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para buscar usuário ID: [{UsuarioId}]", id);
            
            var idUsuarioLogadoString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idUsuarioLogadoString is null)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Falha ao obter ID do usuário logado. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    400,
                    stopwatch.ElapsedMilliseconds
                );
                return BadRequest(new { statusCode = 400, message = "Falha ao obter ID do usuário logado." });
            }
            var idUsuarioLogado = int.Parse(idUsuarioLogadoString);
            var isAdmin = User.IsInRole("Administrador") || User.IsInRole("Gerente de Produção");

            if (idUsuarioLogado != id && !isAdmin)
            {
                _logger.LogWarning(
                    "Acesso negado: Usuário [{IdUsuarioLogado}] tentou acessar os dados do usuário [{IdAlvo}] sem permissão.",
                    idUsuarioLogado, id);
                return Forbid();
            }
            _logger.LogInformation("Usuário [{IdUsuarioLogado}] autorizado a acessar dados do usuário [{IdAlvo}].", idUsuarioLogado, id);
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
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> InativarPorIdAsync(int id)
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
        [HttpPut("admin/{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AdminAtualizarUsuarioPorIdAsync(int id, [FromBody] AdminUsuarioUpdateRequestDTO usuarioUpdateRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Admin: Requisição recebida para atualizar usuário ID: [{UsuarioId}]", id);

            try
            {
                var usuarioAtualizado = await _userService.AdminAtualizarUsuarioAsync(id, usuarioUpdateRequestDTO);

                if (usuarioAtualizado is null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                    "Admin: Tentativa de atualizar usuário não existente ID: [{UsuarioId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
                    "Admin Usuário atualizado com sucesso: [{UsuarioId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> AtualizarUsuarioPorIdAsync(int id, [FromBody] UsuarioUpdateRequestDTO usuarioUpdateRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Usuario: Requisição recebida para atualizar usuário ID: [{UsuarioId}]", id);

            try
            {
                var idUsuarioLogadoClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(idUsuarioLogadoClaim, out var idUsuarioLogado))
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                        "Usuario: Falha ao obter ID do usuário logado. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                        HttpContext.Request.Method,
                        HttpContext.Request.Path,
                        400,
                        stopwatch.ElapsedMilliseconds
                    );
                    return BadRequest(new { statusCode = 400,message = "Falha ao obter ID do usuário logado." });
                }

                var usuarioAtualizado = await _userService.AtualizarUsuarioAsync(idUsuarioLogado, id,usuarioUpdateRequestDTO);

                if (usuarioAtualizado is null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                        "Usuario: Tentativa de atualizar usuário não existente ID: [{UsuarioId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                        idUsuarioLogado,
                        HttpContext.Request.Method,
                        HttpContext.Request.Path,
                        404,
                        stopwatch.ElapsedMilliseconds
                    );
                    return NotFound();
                }

                stopwatch.Stop();
                _logger.LogInformation(
                    "Usuario: Usuário atualizado com sucesso: [{UsuarioId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    idUsuarioLogado,
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
                return BadRequest(new { statusCode = 400, message = ex.Message });
            }
        }
    }
}
