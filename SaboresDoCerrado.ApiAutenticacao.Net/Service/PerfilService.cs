using Mapster;
using SaboresDoCerrado.ApiAutenticacao.Net.Model;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Repository;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public class PerfilService:IPerfilService
    {
        private readonly IPerfilRepository _repositorioPerfil;

        
        public PerfilService(IPerfilRepository repositorioPerfil)
        {
            _repositorioPerfil = repositorioPerfil;
        }

        
        public async Task<IEnumerable<PerfilDTO>> ObterTodosAsync()
        {
            var perfisEntidade = await _repositorioPerfil.ObterTodosAsync();
            return perfisEntidade.Adapt<IEnumerable<PerfilDTO>>();
        }
        public async Task<PerfilDTO> ObterPorId(int id)
        {
            var perfilEntidade = await _repositorioPerfil.ObterPorIdAsync(id);
            if (perfilEntidade == null)
            {
                return null;
            }
            var perfilDto = perfilEntidade.Adapt<PerfilDTO>();
            return  perfilDto;
        }
        
    }
}
