using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Perfil;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Service
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO?> GetRoleByIdAsync(int id);
        Task<bool?> DeactivateActivateRolesByIdAsync(int id, bool newStatus);
        Task<RoleDTO?> UpdateRoleByIdAsync(int id, UpdateRolelRequestDTO updateRolelRequestDTO);
        Task<RoleDTO> CreateRoleAsync(PostRoleRequestDTO postRoleRequestDTO);
    }
}