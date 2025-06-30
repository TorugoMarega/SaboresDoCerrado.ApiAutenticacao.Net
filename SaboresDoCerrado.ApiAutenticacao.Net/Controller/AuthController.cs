using Microsoft.AspNetCore.Mvc;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.request;
using SaboresDoCerrado.ApiAutenticacao.Net.Service;
using System.Diagnostics;

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
        public async Task<IActionResult> CadastrarUsuarioAsync(RegistroRequestDTO registroRequestDTO)
        {
            _logger.LogInformation("Requisição recebida para cadastro do usuario [{email}].", registroRequestDTO.Email);
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
                return CreatedAtAction(nameof(UsuarioController.ObterPorId), "Usuario", new { id = usuarioCadastrado.Id }, usuarioCadastrado); ;
            }
            catch (InvalidOperationException ex)
            {
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
    }
}
