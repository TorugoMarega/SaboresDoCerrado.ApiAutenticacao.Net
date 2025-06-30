using Microsoft.EntityFrameworkCore;
using SaboresDoCerrado.ApiAutenticacao.Net.Data;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Repository
{
    public class PerfilRepository : IPerfilRepository
    {
        private readonly ContextoAplicacao _contexto;
        private readonly ILogger<PerfilRepository> _logger;

        public PerfilRepository(ContextoAplicacao contexto, ILogger<PerfilRepository> logger)
        {
            _contexto = contexto;
            _logger = logger;
        }
        public async Task<IEnumerable<PerfilDTO>> ObterTodosAsync()
        {
            _logger.LogInformation("Iniciando busca de todos os perfis no banco de dados");
            var perfis = await _contexto.Perfil
                .AsNoTracking()
                .Select(perfil => new PerfilDTO
                {
                    Id = perfil.Id,
                    Descricao = perfil.Descricao,
                    Nome = perfil.Nome
                })
                .ToListAsync();
            return perfis;
        }

        public async Task<PerfilDTO?> ObterPorIdAsync(int id)
        {
            _logger.LogInformation("Iniciando busca do perfil por ID {perfilID} no banco de dados", id);
            var perfil = await _contexto.Perfil
                .AsNoTracking()
                .Where(perfil => perfil.Id == id)
                .Select(perfil => new PerfilDTO
                {
                    Id = perfil.Id,
                    Descricao = perfil.Descricao,
                    Nome = perfil.Nome
                })
                .FirstOrDefaultAsync();
            return perfil;
        }
    }
}
