using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Perfil;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Repository
{
    public interface IRoleRepository
    {
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO?> GetRoleByIdAsync(int id);
        Task<Role?> GetRoleEntityByIdAsync(int id);
        Task<int> CountRolesAsync(List<int> roleIds);
        Task<bool?> DeactivateActivateRoleAsync(int id, bool newStatus);
        Task<Role?> UpdateRoleByIdAsync(int id, UpdateRolelRequestDTO roleDTO);
        Task<Role?> CreateRolelAsync(Role role);
        Task<bool> ExistsRoleByNameAsync(string Name, int? idToExclude = null);
    }
}
