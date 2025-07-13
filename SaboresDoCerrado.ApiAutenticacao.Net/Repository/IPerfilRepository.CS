using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Repository
{
    public interface IPerfilRepository
    {
        Task<IEnumerable<PerfilDTO>> ObterTodosAsync();
        Task<PerfilDTO?> ObterPorIdAsync(int id);
        Task<int> ContarPerfisExistentesAsync(List<int> perfilIds);
        Task<bool?> InativarAtivarPerfilAsync(int id, bool novoStatus);
        Task<Perfil?> UpdatePerfilPorIdAsync(int id, Perfil perfil);
        Task<Perfil?> CadastraPerfilAsync(Perfil perfil);
    }
}
