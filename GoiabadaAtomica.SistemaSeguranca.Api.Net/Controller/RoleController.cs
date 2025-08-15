using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Perfil;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Controller
{
    [ApiController]
    [Route("api/tenant/{tenantId}/clientsystem/{clientSystemId}/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger; //inject
        public RoleController(IRoleService rolelService, ILogger<RoleController> logger)
        {
            _roleService = rolelService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRolesAsync(int tenantId, int clientSystemId)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para listar todos os perfis.");
            var roles = await _roleService.GetAllRolesAsync(tenantId, clientSystemId);
            stopwatch.Stop();
            _logger.LogInformation(
                "Requisição finalizada. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                HttpContext.Request.Method,
                HttpContext.Request.Path,
                200,
                stopwatch.ElapsedMilliseconds
            );
            return Ok(roles);
        }

        [HttpGet("{roleId}", Name = "GetRoleByIdAsync")]
        public async Task<IActionResult> GetRoleByIdAsync(int tenantId, int clientSystemId, int roleId)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para buscar perfil por ID: [{ID}].", roleId);
            var role = await _roleService.GetRoleByIdAsync(tenantId, clientSystemId, roleId);
            if (role is null)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Perfil não encontrado. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
            return Ok(role);
        }
        [HttpPost]
        public async Task<IActionResult> PostRoleAsync(int tenantId, int clientSystemId, [FromBody] PostRoleRequestDTO postRoleRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {

                _logger.LogInformation("Requisição recebida para cadastrar perfil: [{name}].", postRoleRequestDTO.Name);

                var createdRole = await _roleService.CreateRoleAsync(tenantId, clientSystemId, postRoleRequestDTO);
                if (createdRole is not null)
                {
                    stopwatch.Stop();
                    _logger.LogInformation(
                    "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                     HttpContext.Request.Method,
                     HttpContext.Request.Path,
                    201,
                    stopwatch.ElapsedMilliseconds
                    );
                    return CreatedAtRoute("GetRoleByIdAsync", new { id = createdRole.Id }, createdRole);
                }
                stopwatch.Stop();
                _logger.LogWarning(
                    "Tentativa de cadastrar perfil finalizada sem sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    400,
                    stopwatch.ElapsedMilliseconds
                    );
                return BadRequest(new { mensagem = "Tentativa de cadastrar perfil finalizada sem sucesso" });
            }
            catch (InvalidOperationException ex)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "{msg}. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    ex.GetBaseException().Message,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    409,
                    stopwatch.ElapsedMilliseconds
                    );
                return Conflict(new { mensagem = ex.GetBaseException().Message });
            }
        }

        [HttpDelete("{roleId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeactivateRoleById(int tenantId, int clientSystemId, int roleId)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("Requisição recebida para INATIVAR o perfil ID: [{RoleId}]", roleId);

                await _roleService.DeactivateActivateRolesByIdAsync(tenantId, clientSystemId, roleId, false);

                stopwatch.Stop();
                _logger.LogInformation(
                    "Perfil inativado com sucesso: [{RoleId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    roleId,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    204,
                    stopwatch.ElapsedMilliseconds
                );

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Tentativa de inativar perfil não existente ID: [{RoleId}]. {ErrorMessage}. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    roleId,
                    ex.Message,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    404,
                    stopwatch.ElapsedMilliseconds
                );
                return NotFound(new { mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Tentativa de inativar perfil falhou por regra de negócio para o ID: [{RoleId}]. {ErrorMessage}. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    roleId,
                    ex.Message,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    409,
                    stopwatch.ElapsedMilliseconds
                );
                return Conflict(new { mensagem = ex.Message });
            }

        }

        [HttpPut("{roleId}")]
        public async Task<IActionResult> UpdateRoleByIdAsync(int tenantId, int clientSystemId, int roleId, [FromBody] UpdateRoleRequestDTO updateRolelRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para atualizar o Perfil ID: [{RoleId}]", roleId);

            try
            {
                var updatedRole = await _roleService.UpdateRoleByIdAsync(tenantId, clientSystemId, roleId, updateRolelRequestDTO);

                if (updatedRole is null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                    "Tentativa de atualizar perfil não existente ID: [{RoleId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    roleId,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    404,
                    stopwatch.ElapsedMilliseconds
                    );
                    return NotFound();
                }

                stopwatch.Stop();
                _logger.LogInformation(
                    "Perfil atualizado com sucesso: [{RoleId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    roleId,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    200,
                    stopwatch.ElapsedMilliseconds
                    );
                return Ok(updatedRole);
            }
            catch (InvalidOperationException ex)
            {
                stopwatch.Stop();
                _logger.LogInformation(
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
    }
}
