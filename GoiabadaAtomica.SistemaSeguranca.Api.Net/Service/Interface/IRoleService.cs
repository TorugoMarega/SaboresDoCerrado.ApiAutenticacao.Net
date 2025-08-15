using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Perfil;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync(int tenantId, int clientSystemId);
        Task<RoleDTO?> GetRoleByIdAsync(int tenantId, int clientSystemId, int roleId);
        Task<RoleDTO> DeactivateActivateRolesByIdAsync(int tenantId, int clientSystemId, int roleId, bool newStatus);
        Task<RoleDTO?> UpdateRoleByIdAsync(int tenantId, int clientSystemId, int roleId, UpdateRoleRequestDTO updateRolelRequestDTO);
        Task<RoleDTO> CreateRoleAsync(int tenantId, int clientSystemId, PostRoleRequestDTO postRoleRequestDTO);
        Task<bool> CheckRoleIUseAync(int roleId);
    }
}