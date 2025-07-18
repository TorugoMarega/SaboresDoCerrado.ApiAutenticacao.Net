using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Perfil;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Repository
{
    public interface IPerfilRepository
    {
        Task<IEnumerable<PerfilDTO>> ObterTodosAsync();
        Task<PerfilDTO?> ObterPorIdNoTrackingAsync(int id);
        Task<int> ContarPerfisExistentesAsync(List<int> perfilIds);
        Task<bool?> InativarAtivarPerfilAsync(int id, bool novoStatus);
        Task<Perfil?> UpdatePorIdAsync(int id, UpdatePerfilRequestDTO perfil);
        Task<Perfil?> CadastraPerfilAsync(Perfil perfil);
        Task<bool> ExistsByNomeAsync(string Nome, int? idAExcluir = null);
    }
}
