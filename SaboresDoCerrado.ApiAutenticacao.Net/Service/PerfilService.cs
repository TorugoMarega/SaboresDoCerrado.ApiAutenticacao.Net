using Mapster;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Repository;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public class PerfilService:IPerfilService
    {
        private readonly IPerfilRepository _perfilRepository;

        
        public PerfilService(IPerfilRepository repositorioPerfil)
        {
            _perfilRepository = repositorioPerfil;
        }

        
        public async Task<IEnumerable<PerfilDTO>> ObterTodosAsync()
        {
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
            return  perfilDto;
        }
        
    }
}
