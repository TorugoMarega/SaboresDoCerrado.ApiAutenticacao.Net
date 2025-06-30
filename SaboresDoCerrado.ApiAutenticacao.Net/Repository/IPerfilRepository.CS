using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Repository
{
    public interface IPerfilRepository
    {
        Task<IEnumerable<PerfilDTO>> ObterTodosAsync();
        Task<PerfilDTO?> ObterPorIdAsync(int id);
    }
}
