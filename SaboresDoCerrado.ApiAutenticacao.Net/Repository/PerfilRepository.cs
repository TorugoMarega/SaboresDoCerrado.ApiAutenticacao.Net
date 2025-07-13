using Microsoft.EntityFrameworkCore;
using SaboresDoCerrado.ApiAutenticacao.Net.Data;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;

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
                    Nome = perfil.Nome,
                    Status = perfil.Status
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
                    Nome = perfil.Nome,
                    Status = perfil.Status
                })
                .FirstOrDefaultAsync();
            return perfil;
        }

        public async Task<int> ContarPerfisExistentesAsync(List<int> perfilIds)
        {
            if (perfilIds == null || perfilIds.Count == 0)
            {
                return 0;
            }

            // Conta quantos dos IDs fornecidos existem na tabela de Perfis.
            return await _contexto.Perfil
                .AsNoTracking()
                .CountAsync(p => perfilIds.Contains(p.Id));

        }
        public async Task<Perfil?> UpdatePerfilPorIdAsync(int id, Perfil perfil)
        {
            throw new NotImplementedException();
        }
        public async Task<bool?> InativarAtivarPerfilAsync(int id, bool novoStatus)
        {
            var perfil = await _contexto.Perfil.FirstOrDefaultAsync(p => p.Id == id);
            if (perfil is not null)
            {
                if (perfil.Status.Equals(novoStatus)) return false;

                perfil.Status = novoStatus;
                await _contexto.SaveChangesAsync();
                return true;
            }
            else return null;
        }
        public async Task<Perfil> CadastraPerfilAsync(Perfil perfil)
        {
            await _contexto.AddAsync(perfil);
            await _contexto.SaveChangesAsync();
            return perfil;
        }
    }
}
