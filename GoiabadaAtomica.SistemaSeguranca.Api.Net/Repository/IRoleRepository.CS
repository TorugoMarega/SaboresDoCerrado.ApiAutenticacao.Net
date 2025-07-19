using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Repository
{
    public interface IRoleRepository
    {
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO?> GetRoleByIdAsync(int id);
        Task<Role?> GetRoleEntityByIdAsync(int id);
        Task<int> CountRolesAsync(List<int> roleIds);
        Task<bool> ExistsRoleByNameAsync(string Name, int? idToExclude = null);
        Task<Role> CreateRolelAsync(Role role);
        Task<int> SaveChangesAsync();
        Task<bool> ExistsRoleInUseByIdAsync(int id);
    }
}