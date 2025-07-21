using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Perfil;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO?> GetRoleByIdAsync(int id);
        Task<bool?> DeactivateActivateRolesByIdAsync(int id, bool newStatus);
        Task<RoleDTO?> UpdateRoleByIdAsync(int id, UpdateRolelRequestDTO updateRolelRequestDTO);
        Task<RoleDTO> CreateRoleAsync(PostRoleRequestDTO postRoleRequestDTO);
        Task<bool> CheckRoleIUseAync(int roleId);
    }
}