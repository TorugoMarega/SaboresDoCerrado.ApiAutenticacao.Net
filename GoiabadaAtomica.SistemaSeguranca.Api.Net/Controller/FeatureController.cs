using GoiabadaAtomica.ApiAutenticacao.Net.Controller;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.Feature;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Controller
{
    [Controller]
    [Route("api/tenant/{tenantId}/clientsystem/{clientSystemId}/[Controller]")]
    [Authorize(Roles = "Administrador")]
    public class FeatureController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IFeatureService _featureService;

        public FeatureController(ILogger<AuthController> logger, IFeatureService featureService)
        {
            _logger = logger;
            _featureService = featureService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFeaturesAsync(int tenantId, int clientSystemId)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para listar todas as Features.");
            var clientSystems = await _featureService.GetAllFeaturesByClientSystemIdAsync(tenantId, clientSystemId);
            stopwatch.Stop();
            _logger.LogInformation(
                "Requisição finalizada. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                HttpContext.Request.Method,
                HttpContext.Request.Path,
                200,
                stopwatch.ElapsedMilliseconds
            );
            return Ok(clientSystems);
        }

        [HttpGet("{featureId}", Name = "GetFeatureByIdAsync")]
        public async Task<IActionResult> GetFeatureByIdAsync(int tenantId, int clientSystemId, int featureId)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para buscar a Feature [{FeatureId}] do sistema [{ClientSystemId}].", featureId, clientSystemId);
            var clientSystem = await _featureService.GetFeatureByIdAsync(tenantId, clientSystemId, featureId);
            if (clientSystem is null)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Feature não encontrada. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
            return Ok(clientSystem);
        }

        [HttpPost]
        public async Task<IActionResult> PostFeatureAsync(int tenantId, int clientSystemId, [FromBody] CreateFeatureRequestDTO createFeatureRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {

                _logger.LogInformation("Requisição recebida para cadastrar Feature: [{name}].", createFeatureRequestDTO.Name);

                var createdFeature = await _featureService.CreateFeatureAsync(tenantId, clientSystemId, createFeatureRequestDTO);
                if (createdFeature is not null)
                {
                    stopwatch.Stop();
                    _logger.LogInformation(
                    "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                     HttpContext.Request.Method,
                     HttpContext.Request.Path,
                    201,
                    stopwatch.ElapsedMilliseconds
                    );
                    return CreatedAtRoute("GetFeatureByIdAsync", new { id = createdFeature.Id }, createdFeature);
                }
                stopwatch.Stop();
                _logger.LogWarning(
                    "Tentativa de cadastrar Feature finalizada sem sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    400,
                    stopwatch.ElapsedMilliseconds
                    );
                return BadRequest(new { mensagem = "Tentativa de cadastrar Feature finalizada sem sucesso" });
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

        [HttpDelete("{featureId}")]
        public async Task<IActionResult> DeactivateFeatureById(int tenantId, int clientSystemId, int featureId)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("Requisição recebida para INATIVAR a Feature ID: [{FeatureId}]", featureId);

                var success = await _featureService.DeactivateActivateFeatureAsync(tenantId, clientSystemId, featureId, false);

                if (success is null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                        "Tentativa de inativar Feature não existente ID: [{FeatureId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                        featureId,
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
                        "Tentativa de inativar Feature finalizada sem sucesso para o ID: [{FeatureID}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                        featureId,
                        HttpContext.Request.Method,
                        HttpContext.Request.Path,
                        400,
                        stopwatch.ElapsedMilliseconds
                        );
                    return Conflict(new { mensagem = $"A Feature [{featureId}] já está inativa" });
                }

                stopwatch.Stop();
                _logger.LogInformation(
                    "Feature inativada com sucesso: [{FeatureID}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    featureId,
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
                    "Tentativa de inativar Feature finalizada sem sucesso para o ID: [{FeatureID}]. {msg}. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    featureId,
                    ex.Message,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    400,
                    stopwatch.ElapsedMilliseconds
                    );
                return BadRequest(new { mensagem = ex.Message });
            }
        }
        [HttpPut("{featureId}")]
        public async Task<IActionResult> UpdateFeatureByIdAsync(int tenantId, int clientSystemId, int featureId, [FromBody] UpdateFeatureRequestDTO updateFeatureRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para atualizar Feature ID: [{FeatureID}]", featureId);

            try
            {
                var updatedFeature = await _featureService.UpdateFeatureAsync(tenantId, clientSystemId, featureId, updateFeatureRequestDTO);

                if (updatedFeature is null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                    "Tentativa de atualizar Feature não existente ID: [{FeatureID}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    featureId,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    404,
                    stopwatch.ElapsedMilliseconds
                    );
                    return NotFound();
                }

                stopwatch.Stop();
                _logger.LogInformation(
                    "Feature atualizada com sucesso: [{FeatureID}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    featureId,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    200,
                    stopwatch.ElapsedMilliseconds
                    );
                return Ok(updatedFeature);
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
