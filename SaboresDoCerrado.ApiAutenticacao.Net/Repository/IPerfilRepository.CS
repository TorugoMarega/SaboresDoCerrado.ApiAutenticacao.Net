using SaboresDoCerrado.ApiAutenticacao.Net.Model;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Repository
{
    public interface IPerfilRepository
    {
        Task<IEnumerable<Perfil>> ObterTodosAsync();
    }
}
