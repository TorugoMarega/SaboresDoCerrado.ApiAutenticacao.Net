using Mapster;
using Microsoft.EntityFrameworkCore;
using GoiabadaAtomica.ApiAutenticacao.Net.Data;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Perfil;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Repository
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

        public async Task<PerfilDTO?> ObterPorIdNoTrackingAsync(int id)
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
        public async Task<Perfil?> ObterPorIdAsync(int id)
        {
            _logger.LogInformation("Iniciando busca do perfil por ID {perfilID} no banco de dados", id);
            var perfil = await _contexto.Perfil
                .Where(perfil => perfil.Id == id)
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
        public async Task<bool> ExistsByNomeAsync(string Nome, int? idAExcluir = null)
        {
            IQueryable<Perfil> query = _contexto.Perfil.AsNoTracking();
            query = query.Where(p => p.Nome.ToLower() == Nome.ToLower());
            //caso seja passado um id, ele sera excluido da query 
            if (idAExcluir.HasValue)
            {
                query = query.Where(p => p.Id != idAExcluir.Value);
            }
            return await query.AnyAsync();
        }
        public async Task<Perfil?> UpdatePorIdAsync(int id, UpdatePerfilRequestDTO perfilDTO)
        {
            var entidade = await ObterPorIdAsync(id);
            if (entidade is null)
            {
                return null;
            }
            if (await ExistsByNomeAsync(perfilDTO.Nome, id))
            {
                throw new InvalidOperationException($"Há um perfil em uso com o nome [{perfilDTO.Nome}]");
            }
            perfilDTO.Adapt(entidade);
            await _contexto.SaveChangesAsync();
            return entidade;
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
        public async Task<Perfil?> CadastraPerfilAsync(Perfil perfil)
        {
            if (await ExistsByNomeAsync(perfil.Nome))
            {
                throw new InvalidOperationException($"Há um perfil em uso com o nome [{perfil.Nome}]");
            }
            await _contexto.AddAsync(perfil);
            await _contexto.SaveChangesAsync();
            return perfil;
        }
    }
}
