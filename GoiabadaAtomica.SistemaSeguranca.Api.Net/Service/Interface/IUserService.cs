using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<bool> DeactivateActivateUserAsync(int id, bool status);
        Task<UserDTO?> UpdateUserAdminAsync(int id, UpdateUserAdminRequestDTO updateUserAdminRequestDTO);
        Task<UserDTO?> UpdateUserAsync(int loggedInUser, int idReq, UpdateUserRequestDTO updateUserRequestDTO);
    }
}