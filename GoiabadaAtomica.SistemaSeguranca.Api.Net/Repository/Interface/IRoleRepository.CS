using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface
{
    public interface IRoleRepository
    {
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO?> GetRoleByIdAsync(int id);
        Task<RoleEntity?> GetRoleEntityByIdAsync(int id);
        Task<int> CountRolesAsync(List<int> roleIds);
        Task<bool> ExistsRoleByNameAsync(string Name, int? idToExclude = null);
        Task<RoleEntity> CreateRolelAsync(RoleEntity role);
        Task<int> SaveChangesAsync();
        Task<bool> ExistsRoleInUseByIdAsync(int id);
    }
}