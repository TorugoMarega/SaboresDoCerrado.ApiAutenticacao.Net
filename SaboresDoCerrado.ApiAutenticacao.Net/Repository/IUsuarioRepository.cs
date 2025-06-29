using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Repository
{
    public interface IUsuarioRepository
    {
        Task RegistrarUsuarioAsync(Usuario usuario);
        Task<IEnumerable<UsuarioDTO>> ObterTodosAsync();
        Task<UsuarioDTO> ObterPorIdAsync(int id);
        //UsuarioDTO Login(LoginRequestDTO loginRequestDTO);
    }
}