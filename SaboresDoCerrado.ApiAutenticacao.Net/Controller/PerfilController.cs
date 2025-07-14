using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request.Perfil;
using SaboresDoCerrado.ApiAutenticacao.Net.Service;
using System.Diagnostics;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Controller
{
    [ApiController]
    [Route("perfil")]
    [Authorize(Roles = "Administrador")]
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
            var perfil = await _perfilService.ObterPorIdAsync(id);
            if (perfil is null)
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
            return Ok(perfil);
        }
        [HttpPost]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PostPerfilAsync([FromBody] PostPerfilRequestDTO cadastroPerfilRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            try {
                
                _logger.LogInformation("Requisição recebida para cadastrar perfil: [{perfil}].", cadastroPerfilRequestDTO.Nome);

                var perfilCadastrado = await _perfilService.CadastraPerfilAsync(cadastroPerfilRequestDTO);
                if (perfilCadastrado is not null)
                {
                    stopwatch.Stop();
                    _logger.LogInformation(
                    "Requisição finalizada com sucesso. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                     HttpContext.Request.Method,
                     HttpContext.Request.Path,
                    201,
                    stopwatch.ElapsedMilliseconds
                    );
                    return CreatedAtRoute("GetPerfilPorIdAsync", new { id = perfilCadastrado.Id }, perfilCadastrado);
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
            catch (InvalidOperationException ex) {
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
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> InativarPorId(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para INATIVAR o perfil ID: [{PerfilId}]", id);

            var sucesso = await _perfilService.InativarAtivarPerfilAsync(id, false);

            if (sucesso is null)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Tentativa de inativar perfil não existente ID: [{PerfilId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    id,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    404,
                    stopwatch.ElapsedMilliseconds
                    );
                return NotFound();
            }

            if (sucesso.Equals(false))
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Tentativa de inativar perfil finalizada sem sucesso para o ID: [{PerfilId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                    id,
                    HttpContext.Request.Method,
                    HttpContext.Request.Path,
                    400,
                    stopwatch.ElapsedMilliseconds
                    );
                return BadRequest(new {mensagem = $"O perfil [{id}] já está inativo"});
            }

            stopwatch.Stop();
            _logger.LogInformation(
                "Perfil inativado com sucesso: [{PerfilId}]. Método: {HttpMethod}, Caminho: {Path}, Status: {StatusCode}, Duration: {Duration}ms",
                id,
                HttpContext.Request.Method,
                HttpContext.Request.Path,
                200,
                stopwatch.ElapsedMilliseconds
                );
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerfilPorIdAsync(int id, [FromBody] UpdatePerfilRequestDTO perfilUpdateRequestDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para atualizar usuário ID: [{PerfilId}]", id);

            try
            {
                var perfilAtualizado = await _perfilService.UpdatePerfilPorIdAsync(id, perfilUpdateRequestDTO);

                if (perfilAtualizado is null)
                {
                    stopwatch.Stop();
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
