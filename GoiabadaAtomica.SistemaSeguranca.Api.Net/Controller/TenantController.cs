using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.Tenant;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly ILogger<TenantController> _logger;

        public TenantController(ITenantService tenantService, ILogger<TenantController> logger)
        {
            _tenantService = tenantService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTenantsAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para listar todas as empresas.");
            var tenants = await _tenantService.GetAllTenantAsync();
            stopwatch.Stop();
            _logger.LogInformation(
                "Requisição finalizada. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                HttpContext.Request.Method,
                HttpContext.Request.Path,
                200,
                stopwatch.ElapsedMilliseconds
            );
            return Ok(tenants);
        }

        [HttpGet("{id}", Name = "GetTenantByIdAsync")]
        public async Task<IActionResult> GetTenantByIdAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para buscar a Empresa pelo ID: [{ID}].", id);
            var tenant = await _tenantService.GetTenantByIdAsync(id);
            if (tenant is null)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Empresa não encontrada. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
            return Ok(tenant);
        }

        [HttpPost]
        public async Task<IActionResult> PostTenantAsync([FromBody] CreateTenantRequestDTO postTenantRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {

                _logger.LogInformation("Requisição recebida para cadastrar Empresa: [{name}].", postTenantRequestDTO.Name);

                var createdTenant = await _tenantService.CreateTenantAsync(postTenantRequestDTO);
                if (createdTenant is not null)
                {
                    stopwatch.Stop();
                    _logger.LogInformation(
                    "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                     HttpContext.Request.Method,
                     HttpContext.Request.Path,
                    201,
                    stopwatch.ElapsedMilliseconds
                    );
                    return CreatedAtRoute("GetTenantByIdAsync", new { id = createdTenant.Id }, createdTenant);
                }
                stopwatch.Stop();
                _logger.LogWarning(
                    "Tentativa de cadastrar empresa finalizada sem sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    409,
                    stopwatch.ElapsedMilliseconds
                    );
                return Conflict(new { mensagem = "Tentativa de cadastrar empresa finalizada sem sucesso" });
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateTenantById(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("Requisição recebida para INATIVAR a Empresa ID: [{TenantId}]", id);

                var success = await _tenantService.DeactivateActivateTenantAsync(id, false);

                if (success is null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                        "Tentativa de empresa sistema não existente ID: [{TenantId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
                        "Tentativa de inativar empresa finalizada sem sucesso para o ID: [{TenantID}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                        id,
                        HttpContext.Request.Method,
                        HttpContext.Request.Path,
                        400,
                        stopwatch.ElapsedMilliseconds
                        );
                    return BadRequest(new { mensagem = $"A empresa [{id}] já está inativa" });
                }

                stopwatch.Stop();
                _logger.LogInformation(
                    "Empresa inativada com sucesso: [{TenantID}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    id,
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
                _logger.LogWarning(
                    "Tentativa de inativar empresa finalizada sem sucesso para o ID: [{TenantID}]. {msg}. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    id,
                    ex.Message,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    409,
                    stopwatch.ElapsedMilliseconds
                    );
                return Conflict(new { mensagem = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTenantsByIdAsync(int id, [FromBody] UpdateTenantRequestDTO UpdateTenantRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para atualizar empresa ID: [{TenantID}]", id);

            try
            {
                var updatedTenant = await _tenantService.UpdateTenantAsync(id, UpdateTenantRequestDTO);

                if (updatedTenant is null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                    "Tentativa de atualizar empresa não existente ID: [{TenantID}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
                    "Empresa atualizada com sucesso: [{TenantID}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    id,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    200,
                    stopwatch.ElapsedMilliseconds
                    );
                return Ok(updatedTenant);
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
