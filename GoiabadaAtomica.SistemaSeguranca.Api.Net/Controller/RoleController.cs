using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Perfil;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Controller
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _rolelService;
        private readonly ILogger<RoleController> _logger; //inject
        public RoleController(IRoleService rolelService, ILogger<RoleController> logger)
        {
            _rolelService = rolelService;
            _logger = logger;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para listar todos os perfis.");
            var roles = await _rolelService.GetAllRolesAsync();
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

        [HttpGet("{id}", Name = "GetRoleByIdAsync")]
        public async Task<IActionResult> GetRoleByIdAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para buscar perfil por ID: [{ID}].", id);
            var role = await _rolelService.GetRoleByIdAsync(id);
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
        public async Task<IActionResult> PostRoleAsync([FromBody] PostRoleRequestDTO postRoleRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {

                _logger.LogInformation("Requisição recebida para cadastrar perfil: [{name}].", postRoleRequestDTO.Name);

                var createdUser = await _rolelService.CreateRoleAsync(postRoleRequestDTO);
                if (createdUser is not null)
                {
                    stopwatch.Stop();
                    _logger.LogInformation(
                    "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                     HttpContext.Request.Method,
                     HttpContext.Request.Path,
                    201,
                    stopwatch.ElapsedMilliseconds
                    );
                    return CreatedAtRoute("GetRoleByIdAsync", new { id = createdUser.Id }, createdUser);
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
                    400,
                    stopwatch.ElapsedMilliseconds
                    );
                return BadRequest(new { mensagem = ex.GetBaseException().Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> InactivateById(int id)
        {
                var stopwatch = Stopwatch.StartNew();
            try {
                _logger.LogInformation("Requisição recebida para INATIVAR o perfil ID: [{RoleId}]", id);

                var success = await _rolelService.DeactivateActivateRolesByIdAsync(id, false);

                if (success is null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                        "Tentativa de inativar perfil não existente ID: [{RoleId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                        id,
                        HttpContext.Request.Method,
                        HttpContext.Request.Path,
                        404,
                        stopwatch.ElapsedMilliseconds
                        );
                    return NotFound();
                }

                if (success.Equals(false))
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                        "Tentativa de inativar perfil finalizada sem sucesso para o ID: [{RoleId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                        id,
                        HttpContext.Request.Method,
                        HttpContext.Request.Path,
                        400,
                        stopwatch.ElapsedMilliseconds
                        );
                    return BadRequest(new { mensagem = $"O perfil [{id}] já está inativo" });
                }

                stopwatch.Stop();
                _logger.LogInformation(
                    "Perfil inativado com sucesso: [{RoleId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    id,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    200,
                    stopwatch.ElapsedMilliseconds
                    );
                return NoContent();
            }
            catch(InvalidOperationException ex)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Tentativa de inativar perfil finalizada sem sucesso para o ID: [{RoleId}]. {msg}. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    id,
                    ex.Message,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    400,
                    stopwatch.ElapsedMilliseconds
                    );
                return BadRequest(new { mensagem = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoleByIdAsync(int id, [FromBody] UpdateRolelRequestDTO updateRolelRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para atualizar usuário ID: [{RoleId}]", id);

            try
            {
                var updatedRole = await _rolelService.UpdateRoleByIdAsync(id, updateRolelRequestDTO);

                if (updatedRole is null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                    "Tentativa de atualizar perfil não existente ID: [{RoleId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
                    "Perfil atualizado com sucesso: [{RoleId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    id,
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
               400,
               stopwatch.ElapsedMilliseconds
               );
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
