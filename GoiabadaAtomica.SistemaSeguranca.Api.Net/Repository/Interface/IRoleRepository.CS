using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface
{
    public interface IRoleRepository
    {
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync(int clientSystemId);
        Task<RoleDTO?> GetRoleByIdAsync(int clientSystemId, int roleId);
        Task<RoleEntity?> GetRoleEntityByIdAsync(int clientSystemId, int roleId);
        Task<int> CountRolesAsync(int clientSystemId, List<int> roleIds);
        Task<bool> ExistsRoleByNameAsync(int clientSystemId, string name, int? idToExclude = null);
        Task<RoleEntity> CreateRoleAsync(RoleEntity role);
        Task<int> SaveChangesAsync();
        Task<bool> ExistsRoleInUseByIdAsync(int roleId);
    }
}