using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Repository
{
    public interface IUsuarioRepository
    {
        Task<Usuario> RegistrarUsuarioAsync(Usuario usuario);
        Task<IEnumerable<UsuarioDTO>> ObterTodosUsuariosDtoAsync();
        Task<UsuarioDTO?> ObterUsuarioDtoPorIdAsync(int id);
        Task<Usuario?> ObterUsuarioEntidadePorIdAsync(int id);
        Task<UsuarioDTO?> ObterUsuarioDtoPorNomeAsync(string NomeUsuario);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> NomeUsuarioExistsAsync(string NomeUsuario);
        Task<string?> VerificarConflitoAsync(string NomeUsuario, string Email);
        Task<LoginDTO?> ObterUsuarioDtoLoginAsync(string NomeUsuario);
        Task<bool> InativarAivarUsuarioAsync(int id, bool status);
        Task<bool> EmailExistsInAnotherUserAsync(int id, string Email);
        Task AtualizaEntidadeUsuarioAsync(Usuario usuario);
    }
}