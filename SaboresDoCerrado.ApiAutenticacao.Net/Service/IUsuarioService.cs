using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request.Usuario;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDTO>> ObterTodosAsync();
        Task<UsuarioDTO?> ObterPorIdNoTrackAsync(int Id);
        Task<bool> InativarAtivarUsuarioAsync(int Id, bool Status);
        Task<UsuarioDTO?> AdminAtualizarUsuarioAsync(int Id, AdminUsuarioUpdateRequestDTO adminUsuarioUpdateRequestDTO);
        Task<UsuarioDTO?> AtualizarUsuarioAsync(int idUsuarioLogado, int idReq, UsuarioUpdateRequestDTO usuarioUpdateRequestDTO);
    }
}
