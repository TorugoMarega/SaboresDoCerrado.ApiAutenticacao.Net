using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.Auth;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Controller
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> PostUserAsync([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            _logger.LogInformation("Requisição recebida para cadastro do usuario [{username}].", registrationRequestDTO.Username);
            var stopwatch = Stopwatch.StartNew();
            try
            {

                var createdUser = await _authService.RegisterAsync(registrationRequestDTO);

                stopwatch.Stop();
                _logger.LogInformation(
               "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
               HttpContext.Request.Method,
               HttpContext.Request.Path,
               201,
               stopwatch.ElapsedMilliseconds
               );
                return CreatedAtRoute("GetUserByIdAsync", new { id = createdUser.Id }, createdUser);
            }
            catch (InvalidOperationException ex)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                       "{msg}. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                       ex.Message,
                       HttpContext.Request.Method,
                       HttpContext.Request.Path,
                       409,
                       stopwatch.ElapsedMilliseconds
                   );
                return Conflict(new { message = ex.Message });
            }

        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            _logger.LogInformation("Requisição recebida para login do usuario [{userame}].", loginRequestDTO.Username);
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
                _logger.LogWarning("Falha na autenticação para [{Username}]. Retornando 401 Unauthorized em {Duration}ms.", loginRequestDTO.Username, stopwatch.ElapsedMilliseconds);
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
        [HttpPut("password/change")]
        public async Task<IActionResult> UpdateUserPasswordById([FromBody] UpdateUserPasswordRequestDTO updateUserPasswordRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.LogDebug("Buscando ID a partir do Header da requisicao");
                var loggedInUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogDebug("String ID Usuario: [{id}]", loggedInUserIdString);
                if (loggedInUserIdString is null)
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
                _logger.LogInformation("Requisição recebida para atualizar senha do usuário ID: [{UserId}]", loggedInUserIdString);
                var userUpdated = await _authService.UpdateUserPasswordByIdAsync(int.Parse(loggedInUserIdString), updateUserPasswordRequestDTO);

                if (userUpdated.Equals(false))
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                    "Tentativa de atualizar senha de um usuário não existente ID: [{UserId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    loggedInUserIdString,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    404,
                    stopwatch.ElapsedMilliseconds
                    );
                    return NotFound();
                }

                stopwatch.Stop();
                _logger.LogInformation(
                    "Senha do usuário atualizado com sucesso: [{UserId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    loggedInUserIdString,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    204,
                    stopwatch.ElapsedMilliseconds
                    );
                return NoContent();
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
