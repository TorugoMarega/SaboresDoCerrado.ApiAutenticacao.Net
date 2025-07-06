using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDTO>> ObterTodosAsync();
        Task<UsuarioDTO?> ObterPorIdAsync(int id);
        Task<bool> InativarAtivarUsuarioAsync(int id, bool status);
        Task<UsuarioDTO?> UpdateUsuarioPorId(int id,UsuarioUpdateRequestDTO usuarioUpdateRequestDTO);
    }
}
