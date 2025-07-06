using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Repository
{
    public interface IUsuarioRepository
    {
        Task<Usuario> RegistrarUsuarioAsync(Usuario usuario);
        Task<IEnumerable<UsuarioDTO>> ObterTodosAsync();
        Task<UsuarioDTO?> ObterPorIdNoTrackAsync(int id);
        Task<Usuario?> ObterPorIdAsync(int id);
        Task<UsuarioDTO?> ObterUsuarioPorNomeUsuarioAsync(string NomeUsuario);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> NomeUsuarioExistsAsync(string NomeUsuario);
        Task<string?> VerificarConflitoAsync(string NomeUsuario, string Email);
        Task<LoginDTO?> ObterUsuarioLoginAsync(string NomeUsuario);
        Task<bool> InativarAivarUsuarioAsync(int id, bool status);
        Task<bool> EmailExistsInAnotherUserAsync(int id, string Email);
        Task<UsuarioDTO?> UpdateUsuarioPorId(int id, UsuarioUpdateRequestDTO usuarioUpdateRequestDTO);
    }
}