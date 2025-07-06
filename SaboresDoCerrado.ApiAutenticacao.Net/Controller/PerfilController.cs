using Microsoft.AspNetCore.Mvc;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;
using SaboresDoCerrado.ApiAutenticacao.Net.Service;
using System.Diagnostics;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Controller
{
    [ApiController]
    [Route("perfil")]
    public class PerfilController : ControllerBase
    {
        private readonly IPerfilService _perfilService;
        private readonly ILogger<PerfilController> _logger; //inject
        public PerfilController(IPerfilService perfilService, ILogger<PerfilController> logger)
        {
            _perfilService = perfilService;
            _logger = logger;
        }

        [HttpGet("listar")]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetPerfisAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para listar todos os perfis.");
            var perfis = await _perfilService.ObterTodosAsync();
            stopwatch.Stop();
            _logger.LogInformation(
                "Requisição finalizada. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                HttpContext.Request.Method,
                HttpContext.Request.Path,
                200,
                stopwatch.ElapsedMilliseconds
            );
            return Ok(perfis);
        }

        [HttpGet("{id}", Name = "GetPerfilPorIdAsync")]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetPerfilPorIdAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para buscar perfil por ID: [{ID}].", id);
            var perfil = await _perfilService.ObterPorId(id);
            stopwatch.Stop();
            if (perfil is null)
            {
                _logger.LogWarning(
                    "Perfil não encontrado. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    404,
                    stopwatch.ElapsedMilliseconds
                );
                return NotFound();
            }
            _logger.LogInformation(
                "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                HttpContext.Request.Method,
                HttpContext.Request.Path,
                200,
                stopwatch.ElapsedMilliseconds
            );
            return Ok(perfil);
        }
        [HttpPost]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PostPerfilAsync([FromBody] PerfilDTO cadastroPerfilRequestDTO) {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para cadastrar perfil: [{perfil}].", cadastroPerfilRequestDTO.Nome);
            _logger.LogInformation(
            "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
             HttpContext.Request.Method,
             HttpContext.Request.Path,
            201,
            stopwatch.ElapsedMilliseconds
            );
            
            cadastroPerfilRequestDTO.Id = 1;
            var perfilCadastrado = cadastroPerfilRequestDTO;

            return CreatedAtRoute("GetPerfilPorIdAsync", new { id = perfilCadastrado.Id }, perfilCadastrado);
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdatePerfilAsync(int id,[FromBody] PerfilDTO cadastroPerfilRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para atualizar o perfil: [{perfil}].", id);
            _logger.LogInformation(
            "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
             HttpContext.Request.Method,
             HttpContext.Request.Path,
            200,
            stopwatch.ElapsedMilliseconds
            );

            cadastroPerfilRequestDTO.Id = 1;
            var perfilAtualizado = cadastroPerfilRequestDTO;

            return Ok(perfilAtualizado);
        }
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> InativarPorId(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para INATIVAR o perfil ID: [{PerfilId}]", id);

            var sucesso = await _perfilService.InativarAtivarPerfilAsync(id, false);

            if (!sucesso)
            {
                _logger.LogWarning(
                    "Tentativa de inativar usuário não existente ID: [{PerfilId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
                "Usuário inativado com sucesso: [{PerfilId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                id,
                HttpContext.Request.Method,
                HttpContext.Request.Path,
                200,
                stopwatch.ElapsedMilliseconds
                );
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerfilPorIdAsync(int id, [FromBody] PerfilDTO perfilUpdateRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para atualizar usuário ID: [{PerfilId}]", id);

            try
            {
                var perfilAtualizado = await _perfilService.UpdatePerfilPorIdAsync(id, perfilUpdateRequestDTO);

                if (perfilAtualizado is null)
                {
                    _logger.LogWarning(
                    "Tentativa de atualizar perfil não existente ID: [{PerfilId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
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
                    "Perfil atualizado com sucesso: [{PerfilId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    id,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    200,
                    stopwatch.ElapsedMilliseconds
                    );
                return Ok(perfilAtualizado);
            }
            catch (InvalidOperationException ex)
            {
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
