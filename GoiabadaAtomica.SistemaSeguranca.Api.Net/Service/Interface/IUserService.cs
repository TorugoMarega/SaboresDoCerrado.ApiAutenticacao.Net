using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync(int tenantId);
        Task<UserDTO?> GetUserByIdAsync(int tenantId, int userId);
        Task<bool> DeactivateActivateUserAsync(int tenantId, int userId, bool status);
        Task<UserDTO?> UpdateUserAdminAsync(int tenantId, int userId, UpdateUserAdminRequestDTO updateUserAdminRequestDTO);
        Task<UserDTO?> UpdateUserAsync(int tenantId, int loggedInUser, int idReq, UpdateUserRequestDTO updateUserRequestDTO);
    }
}