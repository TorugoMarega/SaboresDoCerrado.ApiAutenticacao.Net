using SaboresDoCerrado.ApiAutenticacao.Net.Model;
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

        
        public async Task<IEnumerable<Perfil>> ObterTodosAsync()
        {
            
            return await _repositorioPerfil.ObterTodosAsync();
        }
        public async Task<Perfil> ObterPorId(int id)
        {

            return await _repositorioPerfil.ObterPorIdAsync(id);
        }
        
    }
}
