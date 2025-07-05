using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Repository
{
    public interface IUsuarioRepository
    {
        Task<Usuario> RegistrarUsuarioAsync(Usuario usuario);
        Task<IEnumerable<UsuarioDTO>> ObterTodosAsync();
        Task<UsuarioDTO?> ObterPorIdAsync(int id);
        Task<UsuarioDTO?> ObterUsuarioPorNomeUsuarioAsync(string NomeUsuario);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> NomeUsuarioExistsAsync(string NomeUsuario);
        Task<string?> VerificarConflitoAsync(string NomeUsuario, string Email);
        Task<LoginDTO?> ObterUsuarioLoginAsync(string NomeUsuario);
    }
}