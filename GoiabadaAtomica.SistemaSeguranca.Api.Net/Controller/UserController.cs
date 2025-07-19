using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using GoiabadaAtomica.ApiAutenticacao.Net.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Controller
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para listar todos os usuários");
            var users = await _userService.GetAllUsersAsync();
            _logger.LogInformation(
               "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
               HttpContext.Request.Method,
               HttpContext.Request.Path,
               200,
               stopwatch.ElapsedMilliseconds
            );
            return Ok(users);
        }

        [HttpGet("{id}", Name = "GetUserByIdAsync")]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para buscar usuário ID: [{UserId}]", id);

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

            if (loggedInUserId != id && !isAdmin)
            {
                _logger.LogWarning(
                    "Acesso negado: Usuário [{LoggedInUserId}] tentou acessar os dados do usuário [{TargetId}] sem permissão.",
                    loggedInUserId, id);
                return Forbid();
            }
            _logger.LogInformation("Usuário [{LoggedInUserId}] autorizado a acessar dados do usuário [{TargetId}].", loggedInUserId, id);
            var user = await _userService.GetUserByIdAsync(id);
            if (user is null)
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
            return Ok(user);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteByIdAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para INATIVAR usuário ID: [{UserId}]", id);

            var success = await _userService.DeactivateActivateUserAsync(id, false);

            if (!success)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Tentativa de inativar usuário não existente ID: [{UserId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
                "Usuário inativado com sucesso: [{UserId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
        public async Task<IActionResult> UpdateAdminUserByIdAsync(int id, [FromBody] UpdateUserAdminRequestDTO updateUserAdminRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Admin: Requisição recebida para atualizar usuário ID: [{UserId}]", id);

            try
            {
                var updatedUser = await _userService.UpdateUserAdminAsync(id, updateUserAdminRequestDTO);

                if (updatedUser is null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                    "Admin: Tentativa de atualizar usuário não existente ID: [{UserId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
                    "Admin Usuário atualizado com sucesso: [{UserId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    id,
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
        public async Task<IActionResult> UpdateUserByIdAsync(int id, [FromBody] UpdateUserRequestDTO updateUserRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Usuario: Requisição recebida para atualizar usuário ID: [{UserId}]", id);

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

                var updatedUser = await _userService.UpdateUserAsync(loggedInUser, id, updateUserRequestDTO);

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
