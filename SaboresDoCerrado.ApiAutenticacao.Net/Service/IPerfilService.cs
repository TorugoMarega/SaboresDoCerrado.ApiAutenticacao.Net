using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public interface IPerfilService
    {
        Task<IEnumerable<PerfilDTO>> ObterTodosAsync();
        Task<PerfilDTO> ObterPorIdAsync(int id);
        Task<bool?> InativarAtivarPerfilAsync(int id, bool status);
        Task<PerfilDTO> UpdatePerfilPorIdAsync(int id, PerfilDTO perfilUpdateRequestDTO);
        Task<PerfilDTO> CadastraPerfilAsync(PostPerfilRequestDTO postPerfilRequestDTO);
    }
}
