using SaboresDoCerrado.ApiAutenticacao.Net.Model;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public interface IPerfilService
    {
        Task<IEnumerable<Perfil>> ObterTodosAsync();
    }
}
