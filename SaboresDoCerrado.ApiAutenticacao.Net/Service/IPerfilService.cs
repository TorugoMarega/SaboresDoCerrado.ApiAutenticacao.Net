using SaboresDoCerrado.ApiAutenticacao.Net.Model;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public interface IPerfilService
    {
        Task<IEnumerable<PerfilDTO>> ObterTodosAsync();
        Task<PerfilDTO> ObterPorId(int id);
    }
}
