using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using SaboresDoCerrado.ApiAutenticacao.Net.Service;
using System.Diagnostics;
using System.Security.Claims;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Controller
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        [HttpPost("register")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CadastrarUsuarioAsync([FromBody] RegistroRequestDTO registroRequestDTO)
        {
            _logger.LogInformation("Requisição recebida para cadastro do usuario [{usuario}].", registroRequestDTO.NomeUsuario);
            var stopwatch = Stopwatch.StartNew();
            try
            {

                var usuarioCadastrado = await _authService.ResgistrarAsync(registroRequestDTO);
                stopwatch.Stop();
                _logger.LogInformation(
               "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
               HttpContext.Request.Method,
               HttpContext.Request.Path,
               201,
               stopwatch.ElapsedMilliseconds
               );
                return CreatedAtAction(nameof(UsuarioController.ObterPorId), "Usuario", new { id = usuarioCadastrado.Id }, usuarioCadastrado);
            }
            catch (InvalidOperationException ex)
            {
                stopwatch.Stop();
                _logger.LogWarning(
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
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            _logger.LogInformation("Requisição recebida para login do usuario [{usuario}].", loginRequestDTO.NomeUsuario);
            var stopwatch = Stopwatch.StartNew();
            try
            {

                var token = await _authService.LoginAsync(loginRequestDTO);
                
                if (token is not null)
                {
                    stopwatch.Stop();
                    _logger.LogInformation(
                  "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                  HttpContext.Request.Method,
                  HttpContext.Request.Path,
                  200,
                  stopwatch.ElapsedMilliseconds
                  );
                    return Ok(token);
                }
                stopwatch.Stop();
                _logger.LogWarning("Falha na autenticação para {NomeUsuario}. Retornando 401 Unauthorized em {Duration}ms.", loginRequestDTO.NomeUsuario, stopwatch.ElapsedMilliseconds);
                return Unauthorized(new { message = "Usuário e/ou senha inválidos." });
            }
            catch (InvalidOperationException ex)
            {
                stopwatch.Stop();
                _logger.LogWarning(
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
                var usuarioAtualizado = await _authService.UpdateSenhaUsuarioPorIdAsync(int.Parse(idUsuarioLogadoString), usuarioUpdateRequestDTO);

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
