using GoiabadaAtomica.ApiAutenticacao.Net.Controller;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.ClientSystem;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Controller
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ClientSystemController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IClientSystemService _clientSystemService;

        public ClientSystemController(ILogger<AuthController> logger, IClientSystemService clientSystemService)
        {
            _logger = logger;
            _clientSystemService = clientSystemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClientSystemsAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para listar todos os perfis.");
            var clientSystems = await _clientSystemService.GetAllClientSystemAsync();
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

        [HttpGet("{id}", Name = "GetClientSystemByIdAsync")]
        public async Task<IActionResult> GetClientSystemByIdAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para buscar o Sistema pelo ID: [{ID}].", id);
            var clientSystem = await _clientSystemService.GetClientSystemByIdAsync(id);
            if (clientSystem is null)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Sistema não encontrado. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
        public async Task<IActionResult> PostClientSystemAsync([FromBody] CreateClientSystemRequestDTO postClientSystemRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {

                _logger.LogInformation("Requisição recebida para cadastrar sistema: [{name}].", postClientSystemRequestDTO.Name);

                var createdSystem = await _clientSystemService.CreateClientSystemAsync(postClientSystemRequestDTO);
                if (createdSystem is not null)
                {
                    stopwatch.Stop();
                    _logger.LogInformation(
                    "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                     HttpContext.Request.Method,
                     HttpContext.Request.Path,
                    201,
                    stopwatch.ElapsedMilliseconds
                    );
                    return CreatedAtRoute("GetClientSystemByIdAsync", new { id = createdSystem.Id }, createdSystem);
                }
                stopwatch.Stop();
                _logger.LogWarning(
                    "Tentativa de cadastrar sistema finalizada sem sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    400,
                    stopwatch.ElapsedMilliseconds
                    );
                return BadRequest(new { mensagem = "Tentativa de cadastrar sistema finalizada sem sucesso" });
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
        public async Task<IActionResult> DeactivateClientSystemById(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("Requisição recebida para INATIVAR o sistema ID: [{RoleId}]", id);

                var success = await _clientSystemService.DeactivateActivateClientSystemAsync(id, false);

                if (success is null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                        "Tentativa de inativar sistema não existente ID: [{RoleId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
                        "Tentativa de inativar sistema finalizada sem sucesso para o ID: [{ClientSystemID}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                        id,
                        HttpContext.Request.Method,
                        HttpContext.Request.Path,
                        400,
                        stopwatch.ElapsedMilliseconds
                        );
                    return BadRequest(new { mensagem = $"O sistema [{id}] já está inativo" });
                }

                stopwatch.Stop();
                _logger.LogInformation(
                    "Sistema inativado com sucesso: [{ClientSystemID}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    id,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    200,
                    stopwatch.ElapsedMilliseconds
                    );
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Tentativa de inativar sistema finalizada sem sucesso para o ID: [{ClientSystemID}]. {msg}. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
        public async Task<IActionResult> UpdateClientSystemsByIdAsync(int id, [FromBody] UpdateClientSystemRequestDTO UpdateClientSystemRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para atualizar sistema ID: [{ClientSystemID}]", id);

            try
            {
                var updatedClientSystems = await _clientSystemService.UpdateClientSystemAsync(id, UpdateClientSystemRequestDTO);

                if (updatedClientSystems is null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning(
                    "Tentativa de atualizar sistema não existente ID: [{ClientSystemID}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
                    "Sistema atualizado com sucesso: [{ClientSystemID}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    id,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    200,
                    stopwatch.ElapsedMilliseconds
                    );
                return Ok(updatedClientSystems);
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