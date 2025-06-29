using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDTO>> ObterTodosAsync();
        Task<UsuarioDTO> ObterPorIdAsync(int id);
    }
}
