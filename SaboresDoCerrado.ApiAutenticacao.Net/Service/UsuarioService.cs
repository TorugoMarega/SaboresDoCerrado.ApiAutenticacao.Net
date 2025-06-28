using Mapster;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Repository;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public class UsuarioService:IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService (IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<UsuarioDTO>> ObterTodosAsync()
        {
            var usuarioEntidade = await _usuarioRepository.ObterTodosAsync();
            return usuarioEntidade.Adapt<IEnumerable<UsuarioDTO>>();
        }
    }
}
