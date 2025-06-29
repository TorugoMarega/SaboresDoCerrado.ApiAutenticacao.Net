using Mapster;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Repository;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public class PerfilService : IPerfilService
    {
        private readonly IPerfilRepository _perfilRepository;
        private readonly ILogger<PerfilService> _logger;


        public PerfilService(IPerfilRepository repositorioPerfil, ILogger<PerfilService> logger)
        {
            _perfilRepository = repositorioPerfil;
            _logger = logger;
        }


        public async Task<IEnumerable<PerfilDTO>> ObterTodosAsync()
        {
            _logger.LogInformation("Iniciando listagem de Perfis");
            var perfisEntidade = await _perfilRepository.ObterTodosAsync();
            return perfisEntidade.Adapt<IEnumerable<PerfilDTO>>();
        }
        public async Task<PerfilDTO> ObterPorId(int id)
        {
            var perfilEntidade = await _perfilRepository.ObterPorIdAsync(id);
            if (perfilEntidade == null)
            {
                return null;
            }
            var perfilDto = perfilEntidade.Adapt<PerfilDTO>();
            return perfilDto;
        }

    }
}
