 using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Controller
{
    [ApiController]
    [Route("api/tenant/{tenantId}/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("list")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetAllUsersAsync(int tenantId)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para listar todos os usuários");
            var users = await _userService.GetAllUsersAsync(tenantId);
            _logger.LogInformation(
               "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
               HttpContext.Request.Method,
               HttpContext.Request.Path,
               200,
               stopwatch.ElapsedMilliseconds
            );
            return Ok(users);
        }

        [HttpGet("{userId}", Name = "GetUserByIdAsync")]
        public async Task<IActionResult> GetUserByIdAsync(int tenantId, int userId)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para buscar usuário ID: [{UserId}]", userId);

            var loggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loggedInUser is null)
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
            var loggedInUserId = int.Parse(loggedInUser);
            var isAdmin = User.IsInRole("Administrador") || User.IsInRole("Gerente de Produção");

            if (loggedInUserId != userId && !isAdmin)
            {
                _logger.LogWarning(
                    "Acesso negado: Usuário [{LoggedInUserId}] tentou acessar os dados do usuário [{TargetId}] sem permissão.",
                    loggedInUserId, userId);
                return Forbid();
            }
            _logger.LogInformation("Usuário [{LoggedInUserId}] autorizado a acessar dados do usuário [{TargetId}].", loggedInUserId, userId);
            var user = await _userService.GetUserByIdAsync(tenantId, userId);
            if (user is null)
            {
                stopwatch.Stop();
                _logger.LogWarning(
               "Usuário não encontrado na base: [{id}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
               userId,
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
            return Ok(user);
        }
        [HttpDelete("{userId}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteByIdAsync(int tenantId, int userId)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para INATIVAR usuário ID: [{UserId}]", userId);

            var success = await _userService.DeactivateActivateUserAsync(tenantId, userId, false);

            if (!success)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Tentativa de inativar usuário não existente ID: [{UserId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    userId,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    404,
                    stopwatch.ElapsedMilliseconds
                    );
                return NotFound();
            }

            stopwatch.Stop();
            _logger.LogInformation(
                "Usuário inativado com sucesso: [{UserId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                userId,
                HttpContext.Request.Method,
                HttpContext.Request.Path,
                204,
                stopwatch.ElapsedMilliseconds
                );
            return NoContent();
        }
        [HttpPut("admin/{userId}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateAdminUserByIdAsync(int tenantId, int userId, [FromBody] UpdateUserAdminRequestDTO updateUserAdminRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Admin: Requisição recebida para atualizar usuário ID: [{UserId}]", userId);

            try
            {
                var updatedUser = await _userService.UpdateUserAdminAsync(tenantId, userId, updateUserAdminRequestDTO);

                if (updatedUser is null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                    "Admin: Tentativa de atualizar usuário não existente ID: [{UserId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    userId,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    404,
                    stopwatch.ElapsedMilliseconds
                    );
                    return NotFound();
                }

                stopwatch.Stop();
                _logger.LogInformation(
                    "Admin Usuário atualizado com sucesso: [{UserId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    userId,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    200,
                    stopwatch.ElapsedMilliseconds
                    );
                return Ok(updatedUser);
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
        public async Task<IActionResult> UpdateUserByIdAsync(int tenantId, int userId, [FromBody] UpdateUserRequestDTO updateUserRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Usuario: Requisição recebida para atualizar usuário ID: [{UserId}]", userId);

            try
            {
                var loggedInUserClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(loggedInUserClaim, out var loggedInUser))
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                        "Usuario: Falha ao obter ID do usuário logado. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                        HttpContext.Request.Method,
                        HttpContext.Request.Path,
                        400,
                        stopwatch.ElapsedMilliseconds
                    );
                    return BadRequest(new { statusCode = 400, message = "Falha ao obter ID do usuário logado." });
                }

                var updatedUser = await _userService.UpdateUserAsync(tenantId, loggedInUser, userId, updateUserRequestDTO);

                if (updatedUser is null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                        "Usuario: Tentativa de atualizar usuário não existente ID: [{UserId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                        loggedInUser,
                        HttpContext.Request.Method,
                        HttpContext.Request.Path,
                        404,
                        stopwatch.ElapsedMilliseconds
                    );
                    return NotFound();
                }

                stopwatch.Stop();
                _logger.LogInformation(
                    "Usuario: Usuário atualizado com sucesso: [{UserId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    loggedInUser,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    200,
                    stopwatch.ElapsedMilliseconds
                );
                return Ok(updatedUser);
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
