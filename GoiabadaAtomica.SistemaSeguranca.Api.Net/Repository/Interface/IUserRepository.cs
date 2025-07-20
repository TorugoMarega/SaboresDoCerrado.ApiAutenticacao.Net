using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface
{
    public interface IUserRepository
    {
        Task<UserEntity> RegisterUserAsync(UserEntity user);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<UserEntity?> GetUserEntityByIdAsync(int id);
        Task<UserEntity?> GetUserByUsernameAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> DeactivateActivateUserAsync(int id, bool status);
        Task<bool> EmailExistsInAnotherUserAsync(int id, string email);
        Task UpdateUserAsync(UserEntity user);
    }
}