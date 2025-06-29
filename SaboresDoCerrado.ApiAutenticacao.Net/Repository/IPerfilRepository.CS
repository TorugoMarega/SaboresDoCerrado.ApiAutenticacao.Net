using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Repository
{
    public interface IPerfilRepository
    {
        Task<IEnumerable<PerfilDTO>> ObterTodosAsync();
        Task<PerfilDTO?> ObterPorIdAsync(int id);
    }
}
