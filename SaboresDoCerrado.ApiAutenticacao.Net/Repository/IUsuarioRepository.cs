using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.request;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Repository
{
    public interface IUsuarioRepository
    {
        Task RegistrarUsuarioAsync(Usuario usuario);
        Task<IEnumerable<Usuario>> ObterTodosAsync();
        //UsuarioDTO Login(LoginRequestDTO loginRequestDTO);
    }
}