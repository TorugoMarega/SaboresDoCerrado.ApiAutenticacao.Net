using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Repository
{
    public interface IPerfilRepository
    {
        Task<IEnumerable<Perfil>> ObterTodosAsync();
        Task<Perfil> ObterPorIdAsync(int id);
    }
}
