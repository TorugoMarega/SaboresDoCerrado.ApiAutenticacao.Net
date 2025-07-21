using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(int Id);
        Task<bool> DeactivateActivateUserAsync(int Id, bool status);
        Task<UserDTO?> UpdateUserAdminAsync(int Id, UpdateUserAdminRequestDTO updateUserAdminRequestDTO);
        Task<UserDTO?> UpdateUserAsync(int loggedInUser, int idReq, UpdateUserRequestDTO updateUserRequestDTO);
    }
}