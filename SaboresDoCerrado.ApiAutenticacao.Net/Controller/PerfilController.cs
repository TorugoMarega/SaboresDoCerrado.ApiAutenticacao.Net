using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerfilPorIdAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Requisição recebida para buscar perfil por ID: {ID}.", id);
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
    }
}
