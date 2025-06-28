using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Repository;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDTO>> ObterTodosAsync();
    }
}
