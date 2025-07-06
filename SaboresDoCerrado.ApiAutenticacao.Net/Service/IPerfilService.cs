using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.entity;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public interface IPerfilService
    {
        Task<IEnumerable<PerfilDTO>> ObterTodosAsync();
        Task<PerfilDTO> ObterPorId(int id);
        Task<bool> InativarAtivarPerfilAsync(int id, bool novoStatus);
        Task <PerfilDTO>UpdatePerfilPorIdAsync(int id, PerfilDTO perfilUpdateRequestDTO);
    }
}
