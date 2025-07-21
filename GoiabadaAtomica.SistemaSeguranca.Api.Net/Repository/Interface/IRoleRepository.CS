using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface
{
    public interface IRoleRepository
    {
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO?> GetRoleByIdAsync(int id);
        Task<RoleEntity?> GetRoleEntityByIdAsync(int id);
        Task<int> CountRolesAsync(List<int> roleIds);
        Task<bool> ExistsRoleByNameAsync(string name, int? idToExclude = null);
        Task<RoleEntity> CreateRolelAsync(RoleEntity role);
        Task<int> SaveChangesAsync();
        Task<bool> ExistsRoleInUseByIdAsync(int id);
    }
}